using Himu.EntityFramework.Core.Entity;

namespace Himu.Home.HttpApi.Response
{
    public class HimuUserBrief
    {
        public string UserName { get; set; } = default!;

        public string AvatarUri { get; set; } = default!;
    }

    public class HimuUserBriefResponse : HimuApiResponse<HimuUserBrief>
    {
        public HimuUserBriefResponse()
        {
        }
    }
}