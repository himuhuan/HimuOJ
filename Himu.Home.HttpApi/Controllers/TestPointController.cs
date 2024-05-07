using Himu.EntityFramework.Core;
using Himu.EntityFramework.Core.Entity;
using Himu.HttpApi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Himu.Home.HttpApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class TestPointController : ControllerBase
    {
        private readonly HimuMySqlContext _context;
        private readonly ILogger<TestPointController> _logger;
        private readonly IWebHostEnvironment _environment;

        public TestPointController(HimuMySqlContext context,
                                   ILogger<TestPointController> logger,
                                   IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
        }

        [HttpPost("problems/{problemId}/test_point")]
        [Authorize]
        public async Task<ActionResult<HimuApiResponse>>
            PostTestPoint(long problemId,
                          IFormFile inputFile,
                          IFormFile answerFile)
        {
            HimuApiResponse response = await DoPostTestPointAuthentication(problemId, User, inputFile, answerFile);
            if (!response.IsSuccess())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("problems/{problemId}/test_points")]
        [Authorize]
        public async Task<ActionResult<HimuApiResponse>>
            PostTestPoints(long problemId,
                           IList<IFormFile> inputFileCollection,
                           IList<IFormFile> answerFileCollection)
        {
            HimuApiResponse response = new();
            if (inputFileCollection.Count != answerFileCollection.Count)
            {
                response.Failed("The number of input files does not match the number of output files.");
                return BadRequest(response);
            }

            for (int i = 0; i < inputFileCollection.Count; i++)
            {
                HimuApiResponse result =
                    await DoPostTestPointAuthentication(problemId, User, inputFileCollection[i], answerFileCollection[i]);
                if (!result.IsSuccess())
                {
                    return BadRequest(result);
                }
            }

            return Ok(response);
        }

        [HttpGet("problems/{problemId}/test_points")]
        public async Task<ActionResult<HimuApiResponse<IEnumerable<long>>>>
            ListTestPoints(long problemId)
        {
            HimuApiResponse<IEnumerable<long>> response = new();

            var query = await _context.ProblemSet
                .Where(p => p.Id == problemId)
                .Include(p => p.TestPoints)
                .SelectMany(p => p.TestPoints)
                .Select(tp => tp.Id).ToListAsync();
            if (query != null)
            {
                response.Value = query;
            }
            return Ok(response);
        }

        [HttpGet("test_points/detail/{testPointId:long}")]
        [Authorize]
        public async Task<ActionResult<HimuTestPointResponse>>
            GetTestPointById(long testPointId)
        {
            HimuTestPointResponse response = new();

            var query = await _context.TestPoints.AsNoTracking()
                                                 .Where(tp => tp.Id == testPointId)
                                                 .SingleOrDefaultAsync();

            // TODO: 优化
            long distributorId = await _context.TestPoints.AsNoTracking()
                                                          .Where(tp => tp.Id == testPointId)
                                                          .Include(tp => tp.Problem)
                                                          .ThenInclude(p => p.Contest)
                                                          .ThenInclude(c => c.Distributor)
                                                          .Select(c => c.Problem.Contest.Distributor.Id)
                                                          .SingleOrDefaultAsync();

            if (query == null)
            {
                response.Failed($"{testPointId} not found");
                return NotFound(response);
            }

            long userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (userId != distributorId)
            {
                response.Failed($"没有对 {query.Problem.Contest.Information.Code}/{query.Problem.Detail.Code} 操作的权限",
                                HimuApiResponseCode.BadAuthentication);
                return Unauthorized(response);
            }

            response.Value = new HimuTestPointResponseValue
            {
                InputFileContent = await System.IO.File.ReadAllTextAsync(Path.Combine(_environment.WebRootPath, query.Input)),
                AnswerFileContent = await System.IO.File.ReadAllTextAsync(Path.Combine(_environment.WebRootPath, query.Expected))
            };
            return Ok(response);
        }

        [HttpDelete("problems/{problemId:long}/test_points")]
        [Authorize]
        public async Task<ActionResult<HimuApiResponse>> RemoveAllTestPoints(long problemId)
        {
            HimuApiResponse response = new();
            // TODO: solve Concurrency Check support
            await _context.TestPoints.Where(tp => tp.Problem.Id == problemId).ExecuteDeleteAsync();
            await _context.SaveChangesAsync();
            return Ok(response);
        }

        [HttpGet("testpoints/{testpointId:long}/input")]
        public async Task<IActionResult> DownloadTestpointInput(long testpointId)
        {
            HimuTestPoint? testPoint = await _context.TestPoints.Where(t => t.Id == testpointId)
                .Include(t => t.Problem)
                .FirstOrDefaultAsync();

            if (testPoint == null)
                return NotFound("Not such a testpoint");
            if (!testPoint.Problem.Detail.AllowDownloadInput)
                return Unauthorized("Download not allowed");
            string filePath = Path.Combine(_environment.WebRootPath, testPoint.Input);
            if (!System.IO.File.Exists(filePath))
                return NotFound("Resource not exists");
            return File(System.IO.File.OpenRead(filePath),
                "application/octet-stream",
                $"{testPoint.CaseName}.in");
        }

        #region private code
        private async Task<HimuApiResponse>
            DoPostTestPointAuthentication(long problemId,
                                          ClaimsPrincipal principal,
                                          IFormFile inputFile,
                                          IFormFile answerFile)
        {
            HimuApiResponse response = new();
            var queryResult = await _context.ProblemSet
                .Where(p => p.Id == problemId)
                .Select(p => new
                {
                    TargetProblem = p,
                    TestPointsCount = p.TestPoints.Count,
                    DistributorId = p.Contest.Distributor.Id,
                })
                .SingleOrDefaultAsync();

            if (queryResult == null)
            {
                response.Failed($"problem {problemId} not found!");
                return response;
            }

            long userId = long.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier));
            if (userId != queryResult.DistributorId)
            {
                response.Failed(
                    $"User {userId} is not authorized to operate on problem {problemId}.",
                    HimuApiResponseCode.BadAuthentication);
                return response;
            }

            int testPointCount = queryResult.TestPointsCount;
            string savedPath = Path.Combine("problems", queryResult.TargetProblem.Id.ToString());
            UploadActionFactory inputUpload = new();
            HimuApiResponse inputResponse = await inputUpload.CheckExtensions(ex => ex == ".in")
                .SetRootPath(_environment.WebRootPath)
                .SaveAs(Path.Combine(savedPath, "input"), testPointCount.ToString())
                .SaveAsync(inputFile);
            if (!inputResponse.IsSuccess())
            {
                return inputResponse;
            }

            UploadActionFactory answerUpload = new();
            HimuApiResponse answerResponse = await answerUpload.CheckExtensions(ex => ex == ".out")
                .SetRootPath(_environment.WebRootPath)
                .SaveAs(Path.Combine(savedPath, "answer"), testPointCount.ToString())
                .SaveAsync(answerFile);
            if (!answerResponse.IsSuccess())
            {
                return answerResponse;
            }

            HimuTestPoint newTestPoint = new()
            {
                Input = inputResponse.Message,
                Expected = answerResponse.Message,
                Problem = queryResult.TargetProblem
            };

            try
            {
                await _context.TestPoints.AddAsync(newTestPoint);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                response.Failed(ex.InnerException!.Message);
                return response;
            }

            _logger.LogInformation(
                "{userId} posted testpoint {testpointId} to {problemId}, files saved",
                userId, newTestPoint.Id, problemId);

            return response;
        }
        #endregion
    }
}