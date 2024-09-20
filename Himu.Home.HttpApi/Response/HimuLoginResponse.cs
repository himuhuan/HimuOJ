using Himu.EntityFramework.Core.Entity;

namespace Himu.Home.HttpApi.Response
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
    }

    public class HimuLoginResponse : HimuApiResponse<HimuLoginResponseValue>
    {
        public HimuLoginResponse()
        {
            Value = new HimuLoginResponseValue();
        }

        public HimuLoginResponse Success(HimuHomeUser user, string accessToken)
        {
            Value.UserId = user.Id;
            Value.AccessToken = accessToken;
            Value.UserName = user.UserName;
            Value.Email = user.Email;
            Value.PhoneNumber = user.PhoneNumber;
            Value.AvatarUri = user.Avatar;
            Value.BackgroundUri = user.Background;
            return this;
        }
    }
}