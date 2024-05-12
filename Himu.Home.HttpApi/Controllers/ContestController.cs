using Himu.EntityFramework.Core;
using Himu.EntityFramework.Core.Entity;
using Himu.HttpApi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Himu.EntityFramework.Core.Entity.Components;
using Himu.HttpApi.Utility.Authorization;

namespace Himu.Home.HttpApi.Controllers
{
    [Route("api/contests")]
    [ApiController]
    public class ContestController : ControllerBase
    {
        private readonly HimuMySqlContext _context;
        private readonly IAuthorizationService _authorizationService;

        public ContestController(HimuMySqlContext context, IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }

        [HttpPost("{contestId}/problems")]
        [Authorize]
        public async Task<ActionResult<HimuApiResponse>>
            PostProblem(long contestId, HimuProblemDetail detail)
        {
            long userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            HimuApiResponse response = new();
            HimuProblem problemToPost = new()
            {
                ContestId = contestId,
                Detail = detail,
                DistributorId = userId
            };

            var authorizationResult 
                = await _authorizationService.AuthorizeAsync(User, problemToPost, HimuCrudOperations.Create);
            if (!authorizationResult.Succeeded)
            {
                response.Failed("Authorization failed: Permission denied", HimuApiResponseCode.BadAuthorization);
                return BadRequest(response);
            }

            try
            {
                await _context.ProblemSet.AddAsync(problemToPost);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                response.Failed(e.InnerException!.Message, HimuApiResponseCode.BadRequest);
                return BadRequest(response);
            }

            return Ok(response);
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
                await _context.Contests.AddAsync(contestToCreate);
                await _context.SaveChangesAsync();
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
        /// We agree that all apis that need to operate the contest must use the contest id.
        /// </summary> 
        [HttpGet("{contestCode}/id")]
        public async Task<ActionResult<HimuApiResponse<long>>>
            GetContestId(string contestCode)
        {
            HimuApiResponse<long> response = new();
            long contestId = await _context.Contests
                                           .AsNoTracking()
                                           .Where(c => c.Information.Code == contestCode)
                                           .Select(c => c.Id)
                                           .SingleOrDefaultAsync();
            if (contestId == 0)
            {
                response.Failed("not such contest");
                return BadRequest(response);
            }

            response.Value = contestId;
            return Ok(response);
        }

        /// <summary>
        /// Get the problem id by contest code and problem code.
        /// We agree that all apis that need to operate the problem must use the problem id.
        /// </summary>
        [HttpGet("{contestCode}/problems/{problemCode}/id")]
        public async Task<ActionResult<HimuApiResponse<long>>> GetProblemId(
            string contestCode,
            string problemCode
        )
        {
            HimuApiResponse<long> response = new();
            var problemId = await _context.ProblemSet
                                          .Include(p => p.Contest)
                                          .AsNoTracking()
                                          .Where(p =>
                                              p.Detail.Code == problemCode &&
                                              p.Contest.Information.Code == contestCode)
                                          .Select(p => p.Id)
                                          .SingleOrDefaultAsync();
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
            bool exist = await _context.ProblemSet
                                       .Where(p =>
                                           p.ContestId == contestId && p.Detail.Code == problemCode)
                                       .AnyAsync();
            if (exist)
            {
                response.Failed("duplicate problem code", HimuApiResponseCode.DuplicateItem);
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}