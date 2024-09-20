using Himu.EntityFramework.Core.Entity;
using Himu.EntityFramework.Core.Entity.Components;
using Himu.Home.HttpApi.Request;
using Himu.Home.HttpApi.Response;
using Himu.Home.HttpApi.Utils;
using System.Diagnostics;
using System.Security.Claims;
using Yitter.IdGenerator;

namespace Himu.Home.HttpApi.Services.Storage.Implement
{
    /// <summary>
    /// The local object storage service.
    /// </summary>
    public class LocalObjectStorageService : IObjectStorageService
    {
        private readonly IWebHostEnvironment _environment;

        public LocalObjectStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public string GetTestPointInputPath(HimuTestPoint testPoint)
            => Path.Combine(_environment.WebRootPath, testPoint.Input);

        /// <remarks> 
        /// Be sure to pay attention to checking for exceptions, 
        /// as this method may throw other exceptions of non-<see cref="HimuApiException"/> due to calling external code. 
        /// </remarks>
        public async Task<HimuTestPoint>
            SaveTestPoint(HimuProblem problem, IFormFile inputFile, IFormFile answerFile, string caseName)
        {
            string savedPath = Path.Combine("problems", problem.Id.ToString());

            FileUploadManager inputUpload = new();
            HimuApiResponse inputResponse = await inputUpload.CheckExtensions(ex => ex == ".in")
                .SetRootPath(_environment.WebRootPath)
                .SaveAs(Path.Combine(savedPath, "input"), caseName)
                .SaveAsync(inputFile);
            if (!inputResponse.IsSuccess())
            {
                throw new HimuApiException(HimuApiResponseCode.StorageFailed, inputResponse.Message);
            }

            FileUploadManager answerUpload = new();
            HimuApiResponse answerResponse = await answerUpload.CheckExtensions(ex => ex == ".out")
                .SetRootPath(_environment.WebRootPath)
                .SaveAs(Path.Combine(savedPath, "answer"), caseName)
                .SaveAsync(answerFile);
            if (!answerResponse.IsSuccess())
            {
                throw new HimuApiException(HimuApiResponseCode.StorageFailed, answerResponse.Message);
            }

            return new HimuTestPoint
            {
                Input = inputResponse.Message,
                Expected = answerResponse.Message,
                Problem = problem
            };
        }

        /// <remarks> 
        /// Be sure to pay attention to checking for exceptions, 
        /// as this method may throw other exceptions of non-<see cref="HimuApiException"/> due to calling external code. 
        /// </remarks>
        public async Task<string>
            SaveUserAvatar(HimuHomeUser user, IFormFile avatarFile)
        {
            var savedPath = Path.Combine("images", user.Id.ToString(), "customization");
            var savedName = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
            FileUploadManager uploadAction = new();
            var result = await uploadAction.MaxFileSize(3_000_000)
                                           .CheckExtensions(DefaultExtensionsChecker.ImageExtensionChecker)
                                           .SetRootPath(_environment.WebRootPath)
                                           .SaveAs(savedPath, savedName)
                                           .SaveAsync(avatarFile);
            if (!result.IsSuccess())
            {
                throw new HimuApiException(HimuApiResponseCode.StorageFailed, result.Message);
            }

            var standardPath = uploadAction.RelativePathWithFullName.Replace("\\", "/");
            return standardPath;
        }

        /// <remarks> 
        /// Be sure to pay attention to checking for exceptions, 
        /// as this method may throw other exceptions of non-<see cref="HimuApiException"/> due to calling external code. 
        /// </remarks>
        public async Task<string>
            SaveUserBackground(HimuHomeUser user, IFormFile backgroundFile)
        {
            var savedPath = Path.Combine("images", user.Id.ToString(), "customization");
            var savedName = "background";
            FileUploadManager uploadAction = new();
            var result = await uploadAction.MaxFileSize(5_000_000)
                                           .CheckExtensions(DefaultExtensionsChecker.ImageExtensionChecker)
                                           .SetRootPath(_environment.WebRootPath)
                                           .SaveAs(savedPath, savedName)
                                           .SaveAsync(backgroundFile);
            if (!result.IsSuccess())
            {
                throw new HimuApiException(HimuApiResponseCode.StorageFailed, result.Message);
            }

            var standardPath = uploadAction.RelativePathWithFullName.Replace("\\", "/");
            return standardPath;
        }

        /// <remarks> 
        /// Be sure to pay attention to checking for exceptions, 
        /// as this method may throw other exceptions of non-<see cref="HimuApiException"/> due to calling external code. 
        /// </remarks>
        public string ResetUserAvatar(HimuHomeUser user)
        {
            var savedPath = Path.Combine(_environment.WebRootPath, user.Avatar);
            if (File.Exists(savedPath))
            {
                File.Delete(savedPath);
            }
            return HimuUserAssetFactory.CreateDefaultAvatar(user);
        }

        /// <remarks> 
        /// Be sure to pay attention to checking for exceptions, 
        /// as this method may throw other exceptions of non-<see cref="HimuApiException"/> due to calling external code. 
        /// </remarks>
        public string ResetUserBackground(HimuHomeUser user)
        {
            var savedPath = Path.Combine(_environment.WebRootPath, user.Background);
            if (File.Exists(savedPath))
            {
                File.Delete(savedPath);
            }
            return HimuUserAssetFactory.CreateDefaultBackground(user);
        }

        public HimuHomeUser CreateUser(UserRegisterRequest request)
        {
            HimuHomeUser user = new()
            {
                UserName = request.UserName,
                Email = request.Mail,
                PhoneNumber = request.PhoneNumber,
                RegisterDate = DateOnly.FromDateTime(DateTime.Now),
            };
            user.Avatar = HimuUserAssetFactory.CreateDefaultAvatar(user);
            user.Background = HimuUserAssetFactory.CreateDefaultBackground(user);
            return user;
        }

        /// <summary>
        /// Saves a commit made by a user.
        /// </summary>
        /// <remarks> 
        /// Be sure to pay attention to checking for exceptions, 
        /// as this method may throw other exceptions of non-<see cref="HimuApiException"/> 
        /// due to calling external code. 
        /// </remarks>
        public async Task<HimuCommit> SaveCommit(ClaimsPrincipal user, HimuProblem targetProblem, CompilerPreset preset,
                                                 string rawCodeText)
        {
            long commitId = YitIdHelper.NextId();
            string extension = preset.SupportedExtensions[..preset.SupportedExtensions.IndexOf(',')];
            string localSavePath = Path.Combine(_environment.WebRootPath, "commits", commitId.ToString());

            if (!Directory.Exists(localSavePath))
            {
                Directory.CreateDirectory(localSavePath);
            }

            string localSaveUri = Path.Combine(localSavePath, $"{commitId}{extension}");
            await using (StreamWriter writer = new(localSaveUri, append: false))
            {
                await writer.WriteAsync(rawCodeText);
            }

            return new HimuCommit
            {
                Id = commitId,
                Problem = targetProblem,
                SourceUri = $"commits/{commitId}/{commitId}{extension}",
                Status = ExecutionStatus.PENDING,
                CompilerPreset = preset,
                UserId = long.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)),
                CommitDate = DateOnly.FromDateTime(DateTime.Now)
            };
        }

        /// <summary>
        /// Saves a commit made by a user.
        /// </summary>
        /// <remarks> 
        /// Be sure to pay attention to checking for exceptions, 
        /// as this method may throw other exceptions of non-<see cref="HimuApiException"/> 
        /// due to calling external code. 
        /// </remarks>
        public async Task<HimuCommit> SaveCommit(ClaimsPrincipal user, HimuProblem targetProblem, IFormFile file,
                                                 CompilerPreset preset)
        {
            long commitId = YitIdHelper.NextId();
            string commitIdString = commitId.ToString();
            string savePath = Path.Combine("commits", commitIdString);

            FileUploadManager saveAction = new();
            var saveActionResult = await saveAction.MaxFileSize(3_000_000)
                .CheckExtensions(DefaultExtensionsChecker.SourceExtensionChecker)
                .SetRootPath(_environment.WebRootPath)
                .SaveAs(savePath, commitIdString)
                .SaveAsync(file);
            if (!saveActionResult.IsSuccess())
            {
                throw new HimuApiException(HimuApiResponseCode.StorageFailed, saveActionResult.Message);
            }

            long userId = long.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
            return new HimuCommit
            {
                Id = commitId,
                Problem = targetProblem,
                SourceUri = saveActionResult.Message,
                Status = ExecutionStatus.PENDING,
                CompilerName = preset.Name,
                UserId = userId,
                CommitDate = DateOnly.FromDateTime(DateTime.Now)
            };
        }
    }
}
