using Himu.EntityFramework.Core.Entity;
using Himu.Home.HttpApi.Request;
using System.Security.Claims;

namespace Himu.Home.HttpApi.Services.Storage
{
    /// <summary>
    /// Object storage service.
    /// </summary>
    /// <remarks>
    /// Store objects in the file system or cloud storage, then build dto objects from them.
    /// </remarks>
    public interface IObjectStorageService
    {
        Task<HimuTestPoint> SaveTestPoint(HimuProblem problem, IFormFile inputFile, IFormFile answerFile,
                                          string caseName);

        string GetTestPointInputPath(HimuTestPoint testPoint);

        /// <summary>
        /// Save user avatar and return the saved path.
        /// </summary>
        Task<string> SaveUserAvatar(HimuHomeUser user, IFormFile avatarFile);
        Task<string> SaveUserBackground(HimuHomeUser user, IFormFile backgroundFile);
        string ResetUserAvatar(HimuHomeUser user);
        string ResetUserBackground(HimuHomeUser user);
        HimuHomeUser CreateUser(UserRegisterRequest request);

        Task<HimuCommit> SaveCommit(ClaimsPrincipal user, HimuProblem targetProblem, CompilerPreset preset, 
            string rawCodeText);
        Task<HimuCommit> SaveCommit(ClaimsPrincipal user, HimuProblem targetProblem, IFormFile file, CompilerPreset preset);
    }
}
