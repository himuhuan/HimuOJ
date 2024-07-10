using Himu.EntityFramework.Core.Entity.Components;
using System.Text.Json.Serialization;

namespace Himu.EntityFramework.Core.Entity
{
    public class ChatMessage
    {
        public long Id { get; set; }
        public ChatMessageType Type { get; set; } = ChatMessageType.Text;
        public string Value { get; set; } = null!;
        public DateTime SendTime { get; set; }

        public Guid SessionId { get; set; }
        [JsonIgnore]
        public UserChatSession Session { get; set; } = null!;

        public long SenderId { get; set; }
        [JsonIgnore]
        public HimuHomeUser Sender { get; set; } = null!;
    }
}
