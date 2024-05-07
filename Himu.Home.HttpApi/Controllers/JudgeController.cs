using Himu.Common.Service;
using Himu.EntityFramework.Core;
using Himu.EntityFramework.Core.Entity;
using Himu.HttpApi.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Himu.Home.HttpApi.Controllers
{
    [Route("api/judge")]
    [ApiController]
    public class JudgeController : ControllerBase
    {
        private readonly ILogger<JudgeController> _logger;
        private readonly HimuMySqlContext _context;
        private readonly IJudgeCoreService _judgeCoreService;

        public JudgeController(ILogger<JudgeController> logger,
                               HimuMySqlContext context,
                               IJudgeCoreService judgeCoreService)
        {
            _logger = logger;
            _context = context;
            _judgeCoreService = judgeCoreService;
        }

        [HttpPost("{commitId:long}")]
        public async Task<ActionResult<HimuApiResponse>> JudgeCommit(long commitId)
        {
            HimuApiResponse response = new();
            HimuCommit? commit = await _context.UserCommits
                .Where(c => c.Id == commitId)
                .SingleOrDefaultAsync();

            if (commit == null)
            {
                response.Failed($"Illegal commit id: {commitId}");
                return NotFound(commitId);
            }
            if (commit.Status != ExecutionStatus.PENDING)
            {
                response.Failed($"commit {commit.Id} already been tested");
                return BadRequest(response);
            }

            int status = await _judgeCoreService.RequestJudgeCommitAsync(commitId);
            if (status != 0)
            {
                response.Failed(
                    $"judge core server responsed with status code: {status}");
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
