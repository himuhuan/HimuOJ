namespace Himu.HttpApi.Utility.Response
{
    public record AuthorizedContestsInfo(long ContestId, string ContestTitle, string ContestCode);

    public class AuthorizedContestsListResponse : HimuApiResponse<List<AuthorizedContestsInfo>>
    {
        public AuthorizedContestsListResponse Success(List<AuthorizedContestsInfo> contests)
        {
            Value = contests;
            return this;
        }
    }
}
