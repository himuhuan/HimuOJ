using Himu.EntityFramework.Core.Entity;

namespace Himu.HttpApi.Utility
{   
    public class HimuUserDetailResponseValue
    {
        public long Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string AvatarUri { get; set; } = string.Empty;

        public string BackgroundUri { get; set; } = string.Empty;

        public DateOnly RegisterDate { get; set; }

        public DateOnly LastLoginDate { get; set; }

        public int TotalCommits { get; set; }
        
        public int ProblemSolved { get; set; }
        
        public int CommitAccepted { get; set; }

        public string Permission { get; set; } = string.Empty;
    }

    public class HimuUserDetailResponse : HimuApiResponse<HimuUserDetailResponseValue>
    {
        public HimuUserDetailResponse()
        {
            Value = new HimuUserDetailResponseValue();
        }
    }
}