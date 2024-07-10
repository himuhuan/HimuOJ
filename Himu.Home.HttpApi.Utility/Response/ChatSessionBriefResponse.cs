namespace Himu.HttpApi.Utility.Response
{
    public class ChatSessionBriefValue
    {
        public Guid SessionId { get; set; }
        public string Name { get; set; } = null!;
        public string LastMessage { get; set; } = null!;
        public string SessionAvatarUrl { get; set; } = null!;
    }

    public class ChatSessionBriefResponse : HimuApiResponse<List<ChatSessionBriefValue>>
    {
        public ChatSessionBriefResponse Success(List<ChatSessionBriefValue> list)
        {
            Value = list;
            return this;
        }
    }
}
