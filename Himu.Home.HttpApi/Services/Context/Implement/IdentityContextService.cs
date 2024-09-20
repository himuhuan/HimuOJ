using Himu.Common.Service;
using Himu.EntityFramework.Core.Contests;
using Himu.EntityFramework.Core.Entity;
using Himu.EntityFramework.Core.Entity.Components;
using Himu.Home.HttpApi.Response;
using Himu.Home.HttpApi.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Himu.Home.HttpApi.Services.Context.Implement
{
    public class IdentityContextService : IIdentityContextService
    {
        private readonly HimuIdentityContext _identityContext;
        private readonly UserManager<HimuHomeUser> _userManager;
        private readonly HimuOnlineJudgeContext _oJContext;
        private readonly IUserJwtManager _userJwtManager;

        public IdentityContextService(HimuIdentityContext identityContext,
                                      UserManager<HimuHomeUser> userManager,
                                      HimuOnlineJudgeContext onlineJudgeContext,
                                      IUserJwtManager userJwtManager)
        {
            _identityContext = identityContext;
            _userManager = userManager;
            _oJContext = onlineJudgeContext;
            _userJwtManager = userJwtManager;
        }

        public async Task AddPermissionRecords(HimuHomeUser user, HimuHomeRole role)
        {
            PermissionRecord record = new()
            {
                UserId = user.Id,
                RoleId = role.Id,
                Date = DateTime.Now,
                Operation = PermissionOperation.Add
            };
            await _identityContext.PermissionRecords.AddAsync(record);
            await _identityContext.SaveChangesAsync();
        }

        public async Task<HimuHomeUser?> FindUser(string loginMethod, string input)
        {
            if (loginMethod == "mail")
                return await _userManager.FindByEmailAsync(input);
            else if (loginMethod == "user")
                return await _userManager.FindByNameAsync(input);
            else
                return null;
        }

        public async Task<TimeSpan?> UserLockedOut(HimuHomeUser user)
        {
            if (!_userManager.SupportsUserLockout || !await _userManager.IsLockedOutAsync(user))
            {
                return null;
            }
            return await _userManager.GetLockoutEndDateAsync(user) - DateTimeOffset.UtcNow;
        }

        public Task<HimuHomeUser> FindUserById(long userId) => _userManager.FindByIdAsync(userId.ToString());

        public Task<HimuHomeUser> FindUserById(string userId) => _userManager.FindByIdAsync(userId);

        public Task<HimuHomeUser> FindUserByName(string userName) => _userManager.FindByNameAsync(userName);

        public Task<HimuHomeUser> FindUserByEmail(string mailAddress) => _userManager.FindByEmailAsync(mailAddress);

        public async Task<HimuUserDetail> GetUserDetail(long userId)
        {
            HimuHomeUser user = await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new HimuApiException(HimuApiResponseCode.ResourceNotExist, "no such user");

            var userPermission =
                HimuHomeRole.GetHighestRole(await _userManager.GetRolesAsync(user));

            return new HimuUserDetail
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AvatarUri = user.Avatar,
                BackgroundUri = user.Background,
                RegisterDate = user.RegisterDate,
                LastLoginDate = user.LastLoginDate,
                TotalCommits = user.TotalCommitCount,
                ProblemSolved = user.AcceptedProblemCount,
                CommitAccepted = user.AcceptedCommitCount,
                Permission = userPermission ?? HimuHomeRole.StandardUser
            };
        }

        public async Task<HimuUserBrief> GetUserBrief(long userId)
        {
            HimuHomeUser user = await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new HimuApiException(HimuApiResponseCode.ResourceNotExist, "no such user");

            return new HimuUserBrief
            {
                UserName = user.UserName,
                AvatarUri = user.Avatar
            };
        }

        public async Task UpdateUser(HimuHomeUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new HimuApiException(HimuApiResponseCode.DbOperationFailed, "update user failed");
            }
        }

        public async Task UpdateUser(HimuHomeUser user, HimuUserDetail newValue)
        {
            user.Email = newValue.Email;
            user.PhoneNumber = newValue.PhoneNumber;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new HimuApiException(HimuApiResponseCode.DbOperationFailed, "update user failed");
            }
        }

        public Task<IdentityResult> AddUser(HimuHomeUser user, string rawPassword)
            => _userManager.CreateAsync(user, rawPassword);

        public Task<string> GenerateEmailConfirmationToken(HimuHomeUser user)
            => _userManager.GenerateEmailConfirmationTokenAsync(user);

        public async Task<string?> CheckUserPassword(HimuHomeUser user, string password)
        {
            if (await _userManager.CheckPasswordAsync(user, password))
            {
                await _userManager.ResetAccessFailedCountAsync(user);
                user.LastLoginDate = DateOnly.FromDateTime(DateTime.Now);
                await _userManager.UpdateAsync(user);
                return await _userJwtManager.GetAccessTokenAsync(user);
            }

            await _userManager.AccessFailedAsync(user);
            return null;
        }

        public async Task InvalidateToken(HimuHomeUser user)
            => await _userJwtManager.InvalidateTokenAsync(user);

        public async Task<string> GeneratePasswordResetToken(HimuHomeUser user)
            => await _userManager.GeneratePasswordResetTokenAsync(user);

        public async Task<IdentityResult> ResetPassword(HimuHomeUser user, string token, string newPassword)
            => await _userManager.ResetPasswordAsync(user, token, newPassword);

        public async Task<IList<string>> GetUserRoles(HimuHomeUser user)
            => await _userManager.GetRolesAsync(user);

        public async Task<string> GetUserHighestRole(HimuHomeUser user)
            => HimuHomeRole.GetHighestRole(await _userManager.GetRolesAsync(user));
    }
}
