using Himu.EntityFramework.Core;
using Himu.EntityFramework.Core.Entity;
using Himu.HttpApi.Utility;
using Himu.HttpApi.Utility.Request;
using Himu.HttpApi.Utility.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Himu.Home.HttpApi.Controllers;

[Route("api/problems")]
[ApiController]
public class ProblemController : ControllerBase
{
    private readonly HimuMySqlContext           _context;
    private readonly ILogger<ProblemController> _logger;

    public ProblemController(
        HimuMySqlContext           context,
        ILogger<ProblemController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("{problemId}/detail")]
    public async Task<ActionResult<HimuApiResponse<HimuProblemDetail>>>
        GetProblemDetail(long problemId)
    {
        HimuApiResponse<HimuProblemDetail> response = new();

        HimuProblemDetail? contest = await _context.ProblemSet
            .AsNoTracking()
            .Where(p => p.Id == problemId)
            .Select(p => p.Detail)
            .SingleOrDefaultAsync();

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
        FilterProblemList(int page, int size, [FromQuery] ListProblemFilter? filter)
    {
        HimuProblemListResponse response      = new();
        IQueryable<HimuProblem> query         = _context.ProblemSet.AsNoTracking();
        IQueryable<HimuProblem> filteredQuery = filter?.ApplyFilter(query) ?? query;

        _logger.LogDebug("filteredQuery = {queryString}", filteredQuery.ToQueryString());

        var calculatedResult = await _context.CalculateProblemAccuracy()
            .ToDictionaryAsync(p => p.ProblemId);

        var problemsQuery = filteredQuery
            .OrderBy(p => p.Id)
            .Select(p => new HimuProblemListInfo
            {
                ProblemId = p.Id,
                ProblemTitle = p.Detail.Title,
                ProblemCode = p.Detail.Code,
                ContestTitle = p.Contest.Information.Title,
                ContestCode = p.Contest.Information.Code,
                AcceptedCount = p.ProblemAcceptedCount,
                TotalCommitCount = p.ProblemCommitCount,
                AccuracyRate = calculatedResult[p.Id].AccuracyRate
            })
            .Skip((page - 1) * size)
            .Take(size);

        var problems = await problemsQuery.ToListAsync();
        return response.Success(problems, await filteredQuery.LongCountAsync(), size);
    }
         
    [HttpPut("{problemId}")]
    [Authorize]
    public async Task<ActionResult<HimuApiResponse>>
        UpdateProblem(long problemId, HimuProblem problem)
    {
        bool            exist = problem.Id == problemId && await _context.ProblemSet.AnyAsync(p => p.Id == problemId);
        HimuApiResponse response = new();
        if (!exist)
        {
            response.Failed("No such problem", HimuApiResponseCode.ResourceNotExist);
            return NotFound(response);
        }
        _context.Entry(problem).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return Ok(response);
    }
}