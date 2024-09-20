using Himu.EntityFramework.Core.Entity;
using Himu.Home.HttpApi.Hubs;
using Himu.Home.HttpApi.Services.Context;
using Himu.Home.HttpApi.Services.Judge;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Himu.Home.HttpApi.Controllers
{
    [Route("api/judge")]
    [ApiController]
    public class JudgeController : ControllerBase
    {
        private readonly IOJContextService _oJContextService;
        private readonly IHubContext<JudgeHub> _hub;
        private readonly IJudgeTaskDispatcher _judgeTaskDispatcher;

        public JudgeController(IOJContextService oJContextService, IHubContext<JudgeHub> hub, IJudgeTaskDispatcher judgeTaskDispatcher)
        {
            _hub = hub;
            _judgeTaskDispatcher = judgeTaskDispatcher;
            _oJContextService = oJContextService;
        }

        [HttpPost("{commitId}")]
        public async Task<IActionResult> CommitJudgeTask(long commitId)
        {
            HimuProblem? problem = null;
            HimuCommit? commit = await _oJContextService.GetCommitForJudge(commitId);
            if (commit != null)
                problem = await _oJContextService.GetProblemWithTestPoint(commit.ProblemId);
            if (commit == null || problem == null)
                return BadRequest();
            _ = _judgeTaskDispatcher.DispatchCommitTaskAsync(commit, problem, HttpContext.Connection.Id);
            return Ok();
        }
    }
}
