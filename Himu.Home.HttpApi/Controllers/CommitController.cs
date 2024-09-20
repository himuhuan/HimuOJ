using Himu.EntityFramework.Core.Entity;
using Himu.Home.HttpApi.Request;
using Himu.Home.HttpApi.Response;
using Himu.Home.HttpApi.Services.Context;
using Himu.Home.HttpApi.Services.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Himu.Home.HttpApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class CommitController : ControllerBase
    {
        private readonly IOJContextService _oJContextService;
        private readonly IObjectStorageService _objectStorageService;
        private readonly ILogger<CommitController> _logger;
        private readonly IWebHostEnvironment _environment;
        public CommitController(
            IOJContextService oJContextService,
            IObjectStorageService objectStorageService,
            IWebHostEnvironment environment,
            ILogger<CommitController> logger
        )
        {
            _objectStorageService = objectStorageService;
            _oJContextService = oJContextService;
            _environment = environment;
            _logger = logger;
        }

        [HttpPost("commits/submit_file")]
        [Authorize]
        public async Task<ActionResult<HimuApiResponse<long>>>
            CommitSourceFile(long problemId, IFormFile sourceFile, string compilerPresetName)
        {
            HimuApiResponse<long> response = new();

            HimuProblem? problem = await _oJContextService.GetProblemWithContest(problemId);
            if (problem == null)
            {
                response.Failed($"problem {problemId} not found");
                return BadRequest(response);
            }

            CompilerPreset? preset = await _oJContextService.GetCompilerPreset(compilerPresetName);
            if (preset == null)
            {
                response.Failed($"compiler preset {compilerPresetName} not found");
                return BadRequest(response);
            }

            HimuCommit? commitAdded;
            try
            {
                commitAdded = await _objectStorageService.SaveCommit(User, problem, sourceFile, preset);
                await _oJContextService.AddCommit(commitAdded);
            }
            catch (Exception ex)
            {
                response.Failed(ex);
                return response.ActionResult();
            }

            long commitId = commitAdded.Id;
            response.Value = commitId;
            _logger.LogDebug(
                "commit {commitId} => {problemId} submitted, file {path} saved",
                commitId,
                problemId,
                commitAdded.SourceUri);
            return Ok(response);
        }

        [HttpPost("commits/submit_code")]
        [Authorize]
        public async Task<ActionResult<HimuApiResponse<long>>>
            CommitSourceCode(CommitSourceCodeRequest request)
        {
            HimuApiResponse<long> response = new();

            HimuProblem? problem = await _oJContextService.GetProblemWithContest(request.ProblemId);
            if (problem == null)
            {
                response.Failed($"problem {request.ProblemId} not found");
                return BadRequest(response);
            }
            CompilerPreset? preset = await _oJContextService.GetCompilerPreset(request.CompilerPresetName);
            if (preset == null)
            {
                response.Failed($"compiler preset {request.CompilerPresetName} not found");
                return BadRequest(response);
            }

            HimuCommit? commit;
            try
            {
                commit = await _objectStorageService.SaveCommit(User, problem, preset, request.SourceCode);
                await _oJContextService.AddCommit(commit);
            }
            catch (Exception e)
            {
                response.Failed(e);
                return response.ActionResult();
            }

            long commitId = commit.Id;
            response.Value = commitId;
            _logger.LogDebug(
                "commit {commitId} => {problemId} submitted, file {path} saved",
                commitId,
                request.ProblemId,
                commit.SourceUri);

            return Ok(response);
        }

        [HttpDelete("user/{userId}/commits")]
        [Authorize]
        public async Task<ActionResult<HimuApiResponse>> RemoveAllUserCommits(long userId)
        {
            HimuApiResponse response = new();
            await _oJContextService.RemoveUserAllCommits(userId);
            _logger.LogInformation("user {userId}'s commits has removed permanently", userId);
            return Ok(response);
        }

        [HttpGet("user/{userId}/commits/count")]
        [Authorize]
        public async Task<ActionResult<HimuApiResponse<long>>> GetUserCommitCount(long userId)
        {
            HimuApiResponse<long> response = new()
            {
                Value = await _oJContextService.GetUserCommitCount(userId)
            };
            return Ok(response);
        }

        [HttpGet("commits/{commitId}/detail")]
        public async Task<ActionResult<HimuApiResponse<HimuCommit>>>
            GetCommitResult(long commitId)
        {
            HimuApiResponse<HimuCommit> response = new();

            HimuCommit? commit = await _oJContextService.GetFullCommit(commitId);

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
            FilterCommitList(int page, int size, [FromQuery] ListCommitFilterRequest? filter)
        {
            HimuCommitListResponse response = new();
            List<HimuCommitListInfo> commits = await _oJContextService.GetCommitList(page, size, filter);
            if (commits.Count == 0)
            {
                response.Failed("No data", HimuApiResponseCode.ResourceNotExist);
                return NotFound(response);
            }
            long total = await _oJContextService.GetTotalCommitCount(filter);
            return Ok(response.Success(commits, total, size));
        }
    }
}