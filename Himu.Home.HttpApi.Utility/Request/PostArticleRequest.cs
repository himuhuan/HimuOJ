namespace Himu.HttpApi.Utility.Request
{
    public record PostArticleRequest(string Title, string Content)
    {
        public string? Brief { get; set; }
    }
}