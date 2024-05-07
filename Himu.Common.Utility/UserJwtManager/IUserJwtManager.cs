using Microsoft.AspNetCore.Identity;

namespace Himu.Common.Service
{
    public interface IUserJwtManager
    {
        public Task<string> GetAccessTokenAsync(IdentityUser<long> user);

        public Task InvalidateTokenAsync(IdentityUser<long> user);
    }
}