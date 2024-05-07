using Himu.EntityFramework.Core.Entity;

namespace Himu.HttpApi.Utility.Response
{
    public class HimuUserBriefResponseValue
    {
        public string UserName { get; set; } = default!;

        public string AvatarUri { get; set; } = default!;
    }

    public class HimuUserBriefResponse : HimuApiResponse<HimuUserBriefResponseValue>
    {
        public HimuUserBriefResponse()
        {

        }

        public void GetValue(HimuHomeUser user)
        {
            Value = new HimuUserBriefResponseValue
            {
                UserName = user.UserName,
                AvatarUri = user.Avatar
            };
        }
    }
}
