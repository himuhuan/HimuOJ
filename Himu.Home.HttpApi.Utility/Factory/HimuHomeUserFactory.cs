using Himu.EntityFramework.Core.Entity;

namespace Himu.HttpApi.Utility
{
    public static class HimuHomeUserFactory
    {
        public static HimuHomeUser CreateUserFromRequest(UserRegisterRequest request)
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
            user.Contests = new List<HimuContest>();
            return user;
        }
    }
}