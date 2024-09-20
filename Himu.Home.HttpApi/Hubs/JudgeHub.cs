using Himu.EntityFramework.Core.Entity;
using Himu.Home.HttpApi.Response;
using Himu.Home.HttpApi.Services.Context;
using Himu.Home.HttpApi.Services.Judge;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Himu.Home.HttpApi.Hubs
{
    public class JudgeHub : Hub
    {
        private readonly IOJContextService _oJContextService;
        private readonly IJudgeTaskDispatcher _judgeTaskDispatcher;

        public JudgeHub(IOJContextService oJContextService, IJudgeTaskDispatcher judgeTaskDispatcher)
        {
            _oJContextService = oJContextService;
            _judgeTaskDispatcher = judgeTaskDispatcher;
        }

        public async Task<bool> CommitJudgeTask(long commitId)
        {
            HimuProblem? problem = null;
            HimuCommit? commit = await _oJContextService.GetCommitForJudge(commitId);
            if (commit != null)
                problem = await _oJContextService.GetProblemWithTestPoint(commit.ProblemId);
            if (commit == null || problem == null)
                return false;
            _ = _judgeTaskDispatcher.DispatchCommitTaskAsync(commit, problem, Context.ConnectionId);
            return true;
        }
    }
}
