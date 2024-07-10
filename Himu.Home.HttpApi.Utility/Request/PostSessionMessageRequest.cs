namespace Himu.HttpApi.Utility.Request
{
    public class PostSessionMessageRequest
    {
        public string Message { get; set; } = null!;

        public long SenderId { get; set; }
    }
}
