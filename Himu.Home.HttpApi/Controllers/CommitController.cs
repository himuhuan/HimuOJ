using System.Security.Claims;
using Himu.EntityFramework.Core;
using Himu.EntityFramework.Core.Entity;
using Himu.HttpApi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yitter.IdGenerator;
using Himu.EntityFramework.Core.Entity.Components;
using Himu.HttpApi.Utility.Request;
using Himu.HttpApi.Utility.Response;

namespace Himu.Home.HttpApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class CommitController : ControllerBase
    {
        private readonly HimuMySqlContext _context;
        private readonly ILogger<CommitController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly JudgeController _judgeController;

        public CommitController(
            HimuMySqlContext context,
            IWebHostEnvironment environment,
            ILogger<CommitController> logger,
            JudgeController judgeController
        )
        {
            _context = context;
            _environment = environment;
            _logger = logger;
            _judgeController = judgeController;
        }

        [HttpPost("commits/submit_file")]
        [Authorize]
        public async Task<ActionResult<HimuApiResponse<long>>>
            CommitSourceFile(long problemId, IFormFile sourceFile, string language)
        {
            HimuProblem? problem = await _context.ProblemSet
                                                 .Include(p => p.Contest)
                                                 .Where(p => p.Id == problemId)
                                                 .SingleOrDefaultAsync();

            HimuApiResponse<long> response = new();
            long userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (problem == null)
            {
                response.Failed($"problem {problemId} not found");
                return BadRequest(response);
            }

            long commitId = YitIdHelper.NextId();
            string commitIdString = commitId.ToString();
            string savePath = Path.Combine("commits", commitIdString);
            UploadActionFactory uploadAction = new();
            var uploadResponse = await uploadAction.MaxFileSize(3_000_000)
                                                   .CheckExtensions(DefaultExtensionsChecker.SourceExtensionChecker)
                                                   .SetRootPath(_environment.WebRootPath)
                                                   .SaveAs(savePath, commitIdString)
                                                   .SaveAsync(sourceFile);

            if (!uploadResponse.IsSuccess())
            {
                return BadRequest(response);
            }

            string sourceFilePath = uploadResponse.Message;
            HimuCommit commit = new()
            {
                Id = commitId,
                Problem = problem,
                SourceUri = sourceFilePath,
                Status = ExecutionStatus.PENDING,
                CompilerInformation = CompilerInfo.GetCompilerInfoFromLanguage(language),
                UserId = userId,
                CommitDate = DateOnly.FromDateTime(DateTime.Now)
            };

            try
            {
                await InsertCommitWithTestpoints(commit);
            }
            catch (Exception ex)
            {
                response.Failed(ex.Message);
                _logger.LogError(
                    "fatal error on insert commit {commitId} with testpoints: {message}",
                    commitId,
                    ex.Message);
                return BadRequest(response);
            }

            response.Value = commitId;
            _logger.LogDebug(
                "commit {commitId} => {problemId} submitted, file {path} saved",
                commitId,
                problemId,
                commit.SourceUri);

            if (!_environment.IsDevelopment() && problem.Contest.Information.LaunchTaskAtOnce)
            {
                _logger.LogDebug("launch judge task for commit {commitId}", commitId);
                await _judgeController.JudgeCommit(commitId);
            }

            return Ok(response);
        }

        [HttpPost("commits/submit_code")]
        [Authorize]
        public async Task<ActionResult<HimuApiResponse<long>>>
            CommitSourceCode(CommitSourceCodeRequest request)
        {
            HimuProblem? problem = await _context.ProblemSet
                                                 .Include(p => p.Contest)
                                                 .Where(p => p.Id == request.ProblemId)
                                                 .SingleOrDefaultAsync();

            HimuApiResponse<long> response = new();
            long userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (problem == null)
            {
                response.Failed($"problem {request.ProblemId} not found");
                return BadRequest(response);
            }

            long commitId = YitIdHelper.NextId();
            CompilerInfo compilerInfo = CompilerInfo.GetCompilerInfoFromLanguage(request.Language);
            string localSavePath = Path.Combine(new[]
            {
                _environment.WebRootPath,
                "commits",
                commitId.ToString()
            });
            if (!Directory.Exists(localSavePath))
            {
                Directory.CreateDirectory(localSavePath);
            }

            string localSaveUri = Path.Combine(localSavePath, $"{commitId}.{request.Language}");

            HimuCommit commit = new()
            {
                Id = commitId,
                Problem = problem,
                SourceUri = $"commits/{commitId}/{commitId}.{request.Language}",
                Status = ExecutionStatus.PENDING,
                CompilerInformation = compilerInfo,
                UserId = userId,
                CommitDate = DateOnly.FromDateTime(DateTime.Now)
            };

            await using (StreamWriter writer = new(localSaveUri, append: false))
            {
                await writer.WriteAsync(request.SourceCode);
            }

            try
            {
                await InsertCommitWithTestpoints(commit);
            }
            catch (Exception e)
            {
                response.Failed(e.Message);
                _logger.LogError(
                    "fatal error on insert commit {commitId} with testpoints: {message}",
                    commitId,
                    e.Message);
                return BadRequest(response);
            }

            response.Value = commitId;
            _logger.LogDebug(
                "commit {commitId} => {problemId} submitted, file {path} saved",
                commitId,
                request.ProblemId,
                commit.SourceUri);

            // Launch judge task if not in development environment
            if (!_environment.IsDevelopment() && problem.Contest.Information.LaunchTaskAtOnce)
            {
                _logger.LogDebug("launch judge task for commit {commitId}", commitId);
                await _judgeController.JudgeCommit(commitId);
            }

            return Ok(response);
        }

        [HttpDelete("user/{userId}/commits")]
        [Authorize]
        public async Task<ActionResult<HimuApiResponse>> RemoveAllUserCommits(long userId)
        {
            HimuApiResponse response = new();
            // TODO: solve Concurrency Check support
            await _context.UserCommits.Where(c => c.UserId == userId).ExecuteDeleteAsync();
            await _context.SaveChangesAsync();
            _logger.LogInformation("user {userId}'s commits has removed permanently", userId);
            return Ok(response);
        }

        [HttpGet("user/{userId}/commits/count")]
        [Authorize]
        public async Task<ActionResult<HimuApiResponse<long>>> GetUserCommitCount(long userId)
        {
            HimuApiResponse<long> response = new()
            {
                Value = await _context.UserCommits.Where(c => c.UserId == userId).LongCountAsync()
            };
            return Ok(response);
        }

        [HttpGet("commits/{commitId}/detail")]
        public async Task<ActionResult<HimuApiResponse<HimuCommit>>>
            GetCommitResult(long commitId)
        {
            HimuCommit? commit = await _context.UserCommits
                                               .Where(c => c.Id == commitId)
                                               .Include(c => c.Problem)
                                               .Include(c => c.TestPointResults)
                                               .SingleOrDefaultAsync();

            HimuApiResponse<HimuCommit> response = new();

            if (commit == null)
            {
                return NotFound(response);
            }

            response.Value = commit;
            return Ok(response);
        }

        /// <summary>
        /// Get the list of commits with filter.
        /// </summary>
        [HttpGet("commits")]
        public async Task<ActionResult<HimuCommitListResponse>>
            FilterCommitList(
                int page,
                int size,
                [FromQuery]
                ListCommitFilter? filter
            )
        {
            HimuCommitListResponse response = new();
            var query = _context.UserCommits.AsNoTracking().Include(c => c.Problem);
            IQueryable<HimuCommit>? res =
                filter != null ? filter.ApplyFilter(query) : query;

            if (res == null)
            {
                response.Failed("Invalid filter");
                return BadRequest(response);
            }

            var commits = await res.OrderByDescending(c => c.Id)
                                   .Skip((page - 1) * size)
                                   .Take(size)
                                   .Select(c => HimuCommitListInfo.GetFrom(c))
                                   .ToListAsync();

            if (commits.Count == 0)
            {
                response.Failed("No data", HimuApiResponseCode.ResourceNotExist);
                return NotFound(response);
            }

            long total = await res.LongCountAsync();
            return Ok(response.Success(commits, total, size));
        }

        #region private code

        private async Task InsertCommitWithTestpoints(HimuCommit commit)
        {
            var testpoints = _context.TestPoints.Where(t => t.ProblemId == commit.ProblemId);
            // ReSharper disable once MethodHasAsyncOverload
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                await _context.UserCommits.AddAsync(commit);
                await foreach (var testpoint in testpoints.AsAsyncEnumerable())
                {
                    await _context.PointResults.AddAsync(new()
                    {
                        TestPoint = testpoint,
                        Commit = commit,
                        TestStatus = ExecutionStatus.PENDING
                    });
                }

                await transaction.CommitAsync();
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        #endregion
    }
}