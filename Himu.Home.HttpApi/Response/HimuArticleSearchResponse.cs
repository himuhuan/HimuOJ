namespace Himu.Home.HttpApi.Response
{
    public record HimuArticleSearchResponseResult(long ArticleId, long AuthorId, string Title);

    public class HimuArticleSearchResponseValue
    {
        /// <summary>
        /// the ArticleId, AuthorId and title of articles
        /// </summary>
        public List<HimuArticleSearchResponseResult> Results { get; set; } = new();
    }

    public class HimuArticleSearchResponse : HimuApiResponse<HimuArticleSearchResponseValue>
    {
        public HimuArticleSearchResponse()
        {
            Value = new();
        }
    }
}