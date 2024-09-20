using Grpc.Core;
using GrpcJudgeClient;
using Himu.EntityFramework.Core.Entity;
using Himu.EntityFramework.Core.Entity.Components;
using Himu.Home.HttpApi.Hubs;
using Himu.Home.HttpApi.Services.Context;
using Microsoft.AspNetCore.SignalR;

namespace Himu.Home.HttpApi.Services.Judge.Implement
{
    public class GrpcJudgeTaskDispatcher : IJudgeTaskDispatcher
    {
        private readonly JudgeService.JudgeServiceClient _client;
        private readonly IHubContext<JudgeHub> _hub;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<GrpcJudgeTaskDispatcher> _logger;

        public GrpcJudgeTaskDispatcher(JudgeService.JudgeServiceClient client,
                                       IHubContext<JudgeHub> hub,
                                       IServiceProvider serviceProvider,
                                       ILogger<GrpcJudgeTaskDispatcher> logger)
        {
            _client = client;
            _hub = hub;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task DispatchCommitTaskAsync(HimuCommit commit, HimuProblem targetProblem, string connectionId)
        {
            JudgeTask task = BuildTask(commit, targetProblem);
            using var call = _client.ProcessJudgeTask();
            await call.RequestStream.WriteAsync(task);
            await call.RequestStream.CompleteAsync();
            await _hub.Clients.Client(connectionId).SendAsync("CommitStatus", commit.Id, ConvertProtoStatus(JudgeStatus.Running).ToString());
            _logger.LogInformation("Dispatched task for commit {commitId} from {connectionId}", commit.Id, connectionId);

            await foreach (var response in call.ResponseStream.ReadAllAsync())
            {
                UnpackResult(response, ref commit);
                _logger.LogInformation("Received result for commit {commitId} ({status}) from {connectionId}, updating database...",
                    commit.Id, commit.Status, connectionId);
                using (var scope = _serviceProvider.CreateScope())
                {
                    IOJContextService contextService = scope.ServiceProvider.GetRequiredService<IOJContextService>();
                    await contextService.UpdateCommit(commit);
                }
                await _hub.Clients.Client(connectionId).SendAsync("CommitStatus", commit.Id, commit.Status.ToString());
            }
        }

        private static JudgeTask BuildTask(HimuCommit commit, HimuProblem problem)
        {
            JudgeCompilerInfo compilerInfo = new()
            {
                Timeout = (long)commit.CompilerPreset.Timeout.TotalMilliseconds,
                Command = commit.CompilerPreset.Command,
                Name = commit.CompilerPreset.Name,
            };

            JudgeResourceUsage resourceLimit = new()
            {
                Memory = problem.Detail.MaxMemoryLimitByte,
                Time = (long)problem.Detail.MaxExecuteTimeLimit.TotalMilliseconds,
            };

            JudgeTask task = new()
            {
                CommitId = commit.Id,
                Compiler = compilerInfo,
                SourceUri = commit.SourceUri,
                Limit = resourceLimit,
            };

            var testPoints = problem.TestPoints.Select(tp => new JudgeTestPoint
            {
                TestPointId = tp.Id,
                Input = tp.Input,
                Expected = tp.Expected,
            });

            task.TestPoints.AddRange(testPoints);
            return task;
        }

        private static void UnpackResult(JudgeResult result, ref HimuCommit commit)
        {
            commit.Status = ConvertProtoStatus(result.Status);
            if (commit.Status == ExecutionStatus.COMPILE_ERROR || commit.Status == ExecutionStatus.RUNTIME_ERROR)
                commit.MessageFromCompiler = result.Message;
            // result.TestPointResults[0].Diff.Actual = "";
            Dictionary<long, JudgeTestPointResult> testPointResults = result.TestPointResults.ToDictionary(tp => tp.TestPointId);
            if (commit.TestPointResults == null)
                throw new NullReferenceException("TestPointResults is null");
            foreach (var testPoint in commit.TestPointResults)
            {
                if (testPointResults.TryGetValue(testPoint.TestPointId, out var res))
                {
                    testPoint.TestStatus = ConvertProtoStatus(res.Status);
                    if (testPoint.TestStatus == ExecutionStatus.WRONG_ANSWER)
                        testPoint.Difference = new OutputDifference(res.Diff.Expected, res.Diff.Actual, (int)res.Diff.Pos);
                    if (HasResourceUsage(res.Status))
                        testPoint.Usage = new ResourceUsage(res.Usage.Memory, TimeSpan.FromMilliseconds(res.Usage.Time));
                }
            }
        }

        private static ExecutionStatus ConvertProtoStatus(JudgeStatus status) => status switch
        {
            JudgeStatus.Accepted => ExecutionStatus.ACCEPTED,
            JudgeStatus.WrongAnswer => ExecutionStatus.WRONG_ANSWER,
            JudgeStatus.RuntimeError => ExecutionStatus.RUNTIME_ERROR,
            JudgeStatus.TimeLimitExceeded => ExecutionStatus.TIME_LIMIT_EXCEEDED,
            JudgeStatus.MemoryLimitExceeded => ExecutionStatus.MEMORY_LIMIT_EXCEEDED,
            JudgeStatus.CompileError => ExecutionStatus.COMPILE_ERROR,
            JudgeStatus.InternalError => ExecutionStatus.INTERNAL_ERROR,
            JudgeStatus.Skipped => ExecutionStatus.SKIPPED,
            JudgeStatus.Running => ExecutionStatus.RUNNING,
            _ => ExecutionStatus.PENDING,
        };

        private static bool HasResourceUsage(JudgeStatus status) =>
            status == JudgeStatus.Accepted || status == JudgeStatus.WrongAnswer
            || status == JudgeStatus.MemoryLimitExceeded || status == JudgeStatus.TimeLimitExceeded;
    }
}
