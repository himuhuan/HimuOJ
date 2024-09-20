using Himu.EntityFramework.Core.Entity;
using Himu.Home.HttpApi.Request;
using Himu.Home.HttpApi.Response;
using Himu.Home.HttpApi.Services.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Himu.Home.HttpApi.Controllers
{
    [Route("api/problems")]
    [ApiController]
    public class ProblemController : ControllerBase
    {
        private readonly IOJContextService _oJContextService;
        private readonly ILogger<ProblemController> _logger;

        public ProblemController(
            IOJContextService oJContextService,
            ILogger<ProblemController> logger)
        {
            _oJContextService = oJContextService;
            _logger = logger;
        }

        [HttpGet("{problemId}/detail")]
        public async Task<ActionResult<HimuApiResponse<HimuProblemDetail>>>
            GetProblemDetail(long problemId)
        {
            HimuApiResponse<HimuProblemDetail> response = new();

            HimuProblemDetail? contest = await _oJContextService.GetProblemDetail(problemId);

            if (contest == null)
            {
                response.Failed($"No such problem {problemId}",
                    HimuApiResponseCode.ResourceNotExist);
                return NotFound(response);
            }

            response.Value = contest;
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<HimuProblemListResponse>>
            FilterProblemList(int page, int size, [FromQuery] ListProblemFilterRequest? filter)
        {
            HimuProblemListResponse response = new();
            try
            {
                var problems = await _oJContextService.GetProblemList(page, size, filter);
                var total = await _oJContextService.GetTotalProblemCount(filter);
                response.Success(problems, total, size);
                return response.ActionResult();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get problem list");
                response.Failed("Failed to get problem list", HimuApiResponseCode.BadRequest);
                return response.ActionResult();
            }
        }

        [HttpPut("{problemId}")]
        [Authorize]
        public async Task<ActionResult<HimuApiResponse>>
            UpdateProblem(long problemId, HimuProblem problem)
        {
            bool exist = problem.Id == problemId && await _oJContextService.IsProblemExists(problemId);
            HimuApiResponse response = new();
            if (!exist)
            {
                response.Failed("No such problem", HimuApiResponseCode.ResourceNotExist);
                return NotFound(response);
            }

            try
            {
                await _oJContextService.UpdateProblem(problem);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to update problem");
                response.Failed("Failed to update problem", HimuApiResponseCode.DbOperationFailed);
            }
            return response.ActionResult();
        }
    }
}