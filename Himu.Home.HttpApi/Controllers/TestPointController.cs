using Himu.EntityFramework.Core.Contests;
using Himu.EntityFramework.Core.Entity;
using Himu.Home.HttpApi.Response;
using Himu.Home.HttpApi.Services.Authorization;
using Himu.Home.HttpApi.Services.Context;
using Himu.Home.HttpApi.Services.Storage;
using Himu.Home.HttpApi.Utils;
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
        private readonly IOJContextService _oJContextService;
        private readonly ILogger<TestPointController> _logger;
        private readonly IObjectStorageService _objectStorageService;
        private readonly IAuthorizationService _authorizationService;

        public TestPointController(IOJContextService oJContextService,
                                   ILogger<TestPointController> logger,
                                   IAuthorizationService authorizationService,
                                   IObjectStorageService objectStorageService)
        {
            _oJContextService = oJContextService;
            _logger = logger;
            _authorizationService = authorizationService;
            _objectStorageService = objectStorageService;
        }

        [HttpPost("problems/{problemId}/testpoint")]
        [Authorize]
        public async Task<ActionResult<HimuApiResponse>>
            PostTestPoint(long problemId, IFormFile inputFile, IFormFile answerFile)
        {
            HimuApiResponse response = new();

            HimuProblem? problemToModify = await _oJContextService.GetProblemWithTestPoint(problemId);
            if (problemToModify == null)
            {
                response.Failed($"Problem {problemId} not found.", HimuApiResponseCode.ResourceNotExist);
                return NotFound(response);
            }

            var authorizationResult
                = await _authorizationService.AuthorizeAsync(User, problemToModify, HimuCrudOperations.Update);
            if (!authorizationResult.Succeeded)
            {
                response.Failed("Permission denied.", HimuApiResponseCode.BadAuthorization);
                return BadRequest(response);
            }

            try
            {
                HimuTestPoint testPoint
                    = await _objectStorageService.SaveTestPoint(problemToModify, inputFile, answerFile,
                                                                problemToModify.TestPoints.Count.ToString());
                await _oJContextService.AddTestPoint(problemToModify, testPoint);
            }
            catch (HimuApiException e)
            {
                _logger.LogError(e, "Failed to save test point.");
                response.Failed(e.Message, e.Code);
            }
            return response.ActionResult();
        }
 
        [HttpGet("testpoints/{testpointId:long}/input")]
        public async Task<IActionResult> DownloadTestpointInput(long testpointId)
        {
            HimuTestPoint? testPoint = await _oJContextService.GetTestPointWithProblem(testpointId);
            if (testPoint == null)
                return NotFound("Not such a testpoint");
            if (!testPoint.Problem.Detail.AllowDownloadInput)
                return Unauthorized("Download not allowed");
            string filePath = _objectStorageService.GetTestPointInputPath(testPoint);
            if (!System.IO.File.Exists(filePath))
                return new ObjectResult("Resource not exist") { StatusCode = 500 };
            return File(System.IO.File.OpenRead(filePath), "application/octet-stream", $"{testPoint.CaseName}.in");
        }
    }
}