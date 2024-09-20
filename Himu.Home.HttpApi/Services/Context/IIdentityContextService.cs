using Himu.EntityFramework.Core.Entity;
using Himu.Home.HttpApi.Response;
using Microsoft.AspNetCore.Identity;

namespace Himu.Home.HttpApi.Services.Context
{
    public interface IIdentityContextService
    {
        Task AddPermissionRecords(HimuHomeUser user, HimuHomeRole role);
        Task<IdentityResult> AddUser(HimuHomeUser user, string rawPassword);
        Task<string?> CheckUserPassword(HimuHomeUser user, string password);
        Task<HimuHomeUser?> FindUser(string loginMethod, string input);
        Task<HimuHomeUser> FindUserByEmail(string mailAddress);
        Task<HimuHomeUser> FindUserById(long userId);
        Task<HimuHomeUser> FindUserById(string userId);
        Task<HimuHomeUser> FindUserByName(string userName);
        Task<string> GenerateEmailConfirmationToken(HimuHomeUser user);
        Task<string> GeneratePasswordResetToken(HimuHomeUser user);
        Task<HimuUserBrief> GetUserBrief(long userId);
        Task<HimuUserDetail> GetUserDetail(long userId);
        Task<string> GetUserHighestRole(HimuHomeUser user);
        Task<IList<string>> GetUserRoles(HimuHomeUser user);
        Task InvalidateToken(HimuHomeUser user);
        Task<IdentityResult> ResetPassword(HimuHomeUser user, string token, string newPassword);
        Task UpdateUser(HimuHomeUser user);
        Task UpdateUser(HimuHomeUser user, HimuUserDetail newValue);
        Task<TimeSpan?> UserLockedOut(HimuHomeUser user);
    }
}