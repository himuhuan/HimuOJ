using Himu.EntityFramework.Core.Entity;

namespace Himu.HttpApi.Utility
{
    public class HimuLoginResponseValue
    {
        public long UserId { get; set; }

        public string AccessToken { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string AvatarUri { get; set; } = string.Empty;

        public string BackgroundUri { get; set; } = string.Empty;

        public DateOnly LastLoginDate { get; set; } = DateOnly.MinValue;

        public DateOnly RegisterDate { get; set; } = DateOnly.MinValue;
    }

    public class HimuLoginResponse : HimuApiResponse<HimuLoginResponseValue>
    {
        public HimuLoginResponse()
        {
            Value = new HimuLoginResponseValue();
        }

        public void GetInfo(HimuHomeUser user, string accessToken)
        {
            Value.UserId = user.Id;
            Value.AccessToken = accessToken;
            Value.UserName = user.UserName;
            Value.Email = user.Email;
            Value.PhoneNumber = user.PhoneNumber;
            Value.AvatarUri = user.Avatar;
            Value.BackgroundUri = user.Background;
            Value.LastLoginDate = user.LastLoginDate;
            Value.RegisterDate = user.RegisterDate;
        }
    }
}