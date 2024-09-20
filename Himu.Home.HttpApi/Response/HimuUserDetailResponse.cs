namespace Himu.Home.HttpApi.Response
{
    public class HimuUserDetail
    {
        public long Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string AvatarUri { get; set; } = string.Empty;

        public string BackgroundUri { get; set; } = string.Empty;

        public DateOnly RegisterDate { get; set; }

        public DateOnly LastLoginDate { get; set; }

        public long TotalCommits { get; set; }

        public long ProblemSolved { get; set; }

        public long CommitAccepted { get; set; }

        public string Permission { get; set; } = string.Empty;
    }

    public class HimuUserDetailResponse : HimuApiResponse<HimuUserDetail>
    {
        public HimuUserDetailResponse()
        {
            Value = new HimuUserDetail();
        }
    }
}