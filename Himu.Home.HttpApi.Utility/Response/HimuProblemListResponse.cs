namespace Himu.HttpApi.Utility.Response
{
    public class HimuProblemListInfo
    {
        public long ProblemId { get; set; }

        public string ProblemTitle { get; set; } = null!;

        public string ProblemCode { get; set; } = null!;

        public string ContestCode { get; set; } = null!;

        public string ContestTitle { get; set; } = null!;

        public long AcceptedCount { get; set; } = 0;

        public long TotalCommitCount { get; set; } = 0;
    }

    public class HimuProblemListResponseValue
    {
        public List<HimuProblemListInfo> Problems { get; set; } = null!;
        public long TotalCount { get; set; }
        public int PageCount { get; set; }
    }

    public class HimuProblemListResponse : HimuApiResponse<HimuProblemListResponseValue>
    {
        public HimuProblemListResponse Success(
            List<HimuProblemListInfo> pagedProblems,
            long total,
            int pageSize
        )
        {
            Value = new HimuProblemListResponseValue
            {
                Problems = pagedProblems,
                PageCount = (int)Math.Ceiling(total * 1.0 / pageSize),
                TotalCount = total
            };
            return this;
        }
    }
}