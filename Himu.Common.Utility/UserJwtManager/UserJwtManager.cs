using Himu.EntityFramework.Core.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Himu.Common.Service
{
    public class UserJwtManager : IUserJwtManager
    {
        private readonly JwtOptions _options;
        private readonly IDistributedCache _distributedCache;
        private readonly UserManager<HimuHomeUser> _userManager;

        public UserJwtManager(IOptions<JwtOptions> options, IDistributedCache distributedCache, UserManager<HimuHomeUser> userManager)
        {
            _options = options.Value;
            _distributedCache = distributedCache;
            _userManager = userManager;
        }

        public async Task<string> GetAccessTokenAsync(IdentityUser<long> user)
        {
            string userTokenCacheKey = $"access_token_cache_{user.Id}";
            string? userToken = await _distributedCache.GetStringAsync(userTokenCacheKey);
            if (!string.IsNullOrEmpty(userToken))
            {
                return userToken;
            }

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()!),
                new Claim(ClaimTypes.Name, user.UserName),
            };
            foreach (var role in await _userManager.GetRolesAsync((HimuHomeUser) user))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            byte[] keyBytes = Encoding.UTF8.GetBytes(_options.SecretToken);
            var secretKey = new SymmetricSecurityKey(keyBytes);
            var expire = DateTime.UtcNow.AddSeconds(_options.AccessExpireSeconds);
            SigningCredentials credentials = new(secretKey, SecurityAlgorithms.HmacSha256Signature);
            JwtSecurityToken token = new(claims: claims, expires: expire, signingCredentials: credentials);
            userToken = new JwtSecurityTokenHandler().WriteToken(token);
            await _distributedCache.SetStringAsync(userTokenCacheKey, userToken, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_options.AccessExpireSeconds)
            });
            return userToken;
        }

        public async Task InvalidateTokenAsync(IdentityUser<long> user)
        {
            string userTokenCacheKey = $"access_token_cache_{user.Id}";
            await _distributedCache.RemoveAsync(userTokenCacheKey);
        }
    }
}