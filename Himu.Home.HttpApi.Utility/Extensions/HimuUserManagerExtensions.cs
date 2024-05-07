using Himu.EntityFramework.Core.Entity;
using Microsoft.AspNetCore.Identity;

namespace Himu.HttpApi.Utility
{
    public static class HimuUserManagerExtensions
    {
        public static async Task<HimuHomeUser?>
            FindByMethodAsync(this UserManager<HimuHomeUser> manager, string loginMethod, string input)
        {
            if (loginMethod == "mail")
                return await manager.FindByEmailAsync(input);
            else if (loginMethod == "user")
                return await manager.FindByNameAsync(input);
            else
                return null;
        }
    }
}