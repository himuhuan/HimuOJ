using Himu.EntityFramework.Core.Contests;
using Himu.EntityFramework.Core.Entity;
using Himu.EntityFramework.Core.Entity.Components;
using Himu.Home.HttpApi.Response;
using Himu.Home.HttpApi.Services.Authorization;
using Himu.Home.HttpApi.Services.Context;
using Himu.Home.HttpApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Himu.Home.HttpApi.Controllers
{
    [Route("api/contests")]
    [ApiController]
    public class ContestController : ControllerBase
    {
        private readonly IOJContextService _oJContextService;
        private readonly ILogger<ContestController> _logger;
        private readonly IAuthorizationService _authorizationService;

        public ContestController(IOJContextService oJContextService,
                                 ILogger<ContestController> logger,
                                 IAuthorizationService authorizationService)
        {
            _oJContextService = oJContextService;
            _logger = logger;
            _authorizationService = authorizationService;
        }

        [HttpPost("{contestId}/problems")]
        [Authorize]
        public async Task<ActionResult<HimuApiResponse>>
            PostProblem(long contestId, HimuProblemDetail detail)
        {
            HimuApiResponse response = new();

            HimuProblem problemToPost = new()
            {
                ContestId = contestId,
                Detail = detail,
                DistributorId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))
            };

            var authorizationResult =
                await _authorizationService.AuthorizeAsync(User, problemToPost, HimuCrudOperations.Create);
            if (!authorizationResult.Succeeded)
            {
                response.Failed("Authorization failed: Permission denied", HimuApiResponseCode.BadAuthorization);
                return BadRequest(response);
            }

            try
            {
                await _oJContextService.AddProblem(problemToPost);
            }
            catch (Exception e)
            {
                response.Failed(e.Message, HimuApiResponseCode.DbOperationFailed);
            }

            return response.ActionResult();
        }

        /// <summary>
        /// Post a new contest.
        /// </summary>
        /// <param name="information"> Contest information </param>
        /// <remarks>
        /// TODO: For ContestDistributor, we should send a notification to the administrator.
        /// For now, we just approve the distributor to create a contest directly.
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "ContestDistributor, Administrator")]
        public async Task<ActionResult<HimuApiResponse>> PostHimuContest(
            [FromBody]
            HimuContestInformation information
        )
        {
            HimuApiResponse response = new();
            long userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            HimuContest contestToCreate = new()
            {
                Information = information,
                DistributorId = userId,
                CreateDate = DateOnly.FromDateTime(DateTime.Now)
            };

            var authorizationResult
                = await _authorizationService.AuthorizeAsync(User, contestToCreate, HimuCrudOperations.Create);
            if (!authorizationResult.Succeeded)
            {
                response.Failed("Authorization failed: Permission denied", HimuApiResponseCode.BadAuthorization);
                return BadRequest(response);
            }

            try
            {
                await _oJContextService.AddContest(contestToCreate);
            }
            catch (DbUpdateException e)
            {
                response.Failed(e.InnerException!.Message, HimuApiResponseCode.BadRequest);
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get the contest id by contest code.
        /// We agree that all APIs that need to operate the contest must use the contest id.
        /// </summary>
        [HttpGet("{contestCode}/id")]
        public async Task<ActionResult<HimuApiResponse<long>>>
            GetContestId(string contestCode)
        {
            HimuApiResponse<long> response = new();
            long contestId = await _oJContextService.GetContestIdFromCode(contestCode);

            if (contestId == 0)
            {
                response.Failed("not such contest");
            }
            else
            {
                response.Value = contestId;
            }

            return response.ActionResult();
        }

        /// <summary>
        /// Get the problem id by contest code with problem code.
        /// We agree that all APIs that need to operate the problem must use the problem id.
        /// </summary>
        [HttpGet("{contestCode}/problems/{problemCode}/id")]
        public async Task<ActionResult<HimuApiResponse<long>>> GetProblemId(
            string contestCode,
            string problemCode
        )
        {
            HimuApiResponse<long> response = new();
            var problemId = await _oJContextService.GetProblemIdFromCode(contestCode, problemCode);

            if (problemId == 0)
            {
                response.Failed($"contest {contestCode} do not have {problemCode}",
                    HimuApiResponseCode.ResourceNotExist);
                return NotFound(response);
            }

            response.Value = problemId;
            return Ok(response);
        }

        /// <summary>
        /// Check the problem code is unique in the contest.
        /// </summary>
        [HttpGet("{contestId}/check/problem_code/{problemCode}")]
        public async Task<ActionResult<HimuApiResponse>>
            CheckProblemCode(long contestId, string problemCode)
        {
            HimuApiResponse response = new();
            bool exist = await _oJContextService.IsProblemExists(contestId, problemCode);

            if (exist)
            {
                response.Failed("duplicate problem code", HimuApiResponseCode.DuplicateItem);
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}