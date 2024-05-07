using Himu.EntityFramework.Core;
using Himu.EntityFramework.Core.Entity;
using Himu.HttpApi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Himu.EntityFramework.Core.Entity.Components;

namespace Himu.Home.HttpApi.Controllers
{
    [Route("api/contests")]
    [ApiController]
    public class ContestController : ControllerBase
    {
        private readonly HimuMySqlContext _context;

        public ContestController(HimuMySqlContext context)
        {
            _context = context;
        }

        [HttpPost("{contestId}/problems")]
        [Authorize]
        [HimuActionCheck(HimuActionCheckTargets.ContestDistributor)]
        public async Task<ActionResult<HimuApiResponse>>
            PostProblem(long contestId, HimuProblemDetail detail)
        {
            HimuApiResponse response = new();
            HimuContest? targetContest = await _context.Contests
                                                       .Where(c => c.Id == contestId)
                                                       .Include(c => c.Distributor)
                                                       .SingleOrDefaultAsync();

            long userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Check if the user has permission to operate the contest.
            // Now we only check if the user is the distributor of the contest.
            // TODO: Add different permission levels.
            if (targetContest == null || targetContest.Distributor.Id != userId)
            {
                return BadRequest(response.Failed($"illegal id: {contestId}"));
            }

            // Check if the problem is unique in the contest.
            bool exist = await _context.ProblemSet
                                       .Where(p =>
                                           p.ContestId == contestId && p.Detail.Code == detail.Code)
                                       .AnyAsync();
            if (exist)
            {
                return BadRequest(response.Failed("duplicate problem code", HimuApiResponseCode.DuplicateItem));
            }

            HimuProblem newProblem = new()
            {
                Contest = targetContest,
                Detail = detail,
                DistributorId = userId
            };
            await _context.ProblemSet.AddAsync(newProblem);
            await _context.SaveChangesAsync();
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<HimuApiResponse<ICollection<HimuContestInformation>>>>
            GetAllHimuContests()
        {
            HimuApiResponse<ICollection<HimuContestInformation>> response = new()
            {
                Value = await _context.Contests
                                      .Select(c => c.Information)
                                      .AsNoTracking()
                                      .ToListAsync()
            };

            return Ok(response);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<HimuApiResponse>> CreateHimuContest(
            [FromBody]
            HimuContestInformation information
        )
        {
            HimuApiResponse response = new();

            if (await _context.Contests.AnyAsync(c => c.Information.Code == information.Code))
            {
                response.Failed($"try to re-test {information.Code}");
                return BadRequest(response);
            }

            string distributorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var distributor = await _context.Users
                                            .Where(u => u.Id.ToString() == distributorId)
                                            .SingleAsync();
            HimuContest contest = new(information.Code, information.Title, information.Introduction,
                information.Description)
            {
                Distributor = distributor
            };

            try
            {
                await _context.Contests.AddAsync(contest);
                await _context.SaveChangesAsync();
                return Ok(response);
            }
            catch (DbUpdateException e)
            {
                response.Failed(e.InnerException!.Message);
                return BadRequest(response);
            }
        }

        [HttpDelete("{code}")]
        [Authorize]
        public async Task<ActionResult<HimuApiResponse>> DeleteHimuContest(string code)
        {
            HimuApiResponse response = new();
            long userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            HimuContest? contest = await _context.Contests
                                                 .Where(c => c.Information.Code == code)
                                                 .Include(c => c.Distributor)
                                                 .SingleOrDefaultAsync();

            if (contest == null || contest.Distributor.Id != userId)
            {
                response.Failed($"Contest {code} not found or you don't have permission.");
                return BadRequest(response);
            }

            try
            {
                _context.Contests.Remove(contest);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                response.Failed($"发生并发冲突: {e.InnerException?.Message}");
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
        /// Check user has permission to operate the contest.
        /// </summary>
        /// TODO: Add different permission levels.
        [HttpGet("{contestId}/check/permission")]
        [Authorize]
        public async Task<ActionResult<HimuApiResponse<bool>>>
            CheckContestPermission(long contestId, long userId)
        {
            HimuApiResponse<bool> response = new();
            HimuContest? contest = await _context.Contests
                                                 .Where(c => c.Id == contestId)
                                                 .SingleOrDefaultAsync();
            if (contest == null)
            {
                response.Failed("not such contest");
                return BadRequest(response);
            }

            response.Value = userId == contest.DistributorId;
            return Ok(response);
        }

        /// <summary>
        /// Check the problem code is unique in the contest.
        /// </summary>
        [HttpGet("{contestId}/check/problem_code/{problemCode}")]
        [Authorize]
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