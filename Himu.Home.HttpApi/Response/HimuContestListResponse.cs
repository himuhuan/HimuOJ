namespace Himu.Home.HttpApi.Response;

public class HimuContestListInfo
{
    public long ContestId { get; set; }

    public string ContestCode { get; set; } = null!;

    public string ContestTitle { get; set; } = null!;

    public DateOnly CreateDate { get; set; }
}

public class HimuContestListResponseValue
{
    public List<HimuContestListInfo> Contests { get; set; } = null!;
    public long TotalCount { get; set; }
    public int PageCount { get; set; }
}

public class HimuContestListResponse : HimuApiResponse<HimuContestListResponseValue>
{
    public HimuContestListResponse Success(
        List<HimuContestListInfo> pagedContests,
        long total,
        int pageSize
    )
    {
        Value = new HimuContestListResponseValue
        {
            Contests = pagedContests,
            PageCount = (int) Math.Ceiling(total * 1.0 / pageSize),
            TotalCount = total
        };
        return this;
    }
}