namespace Himu.EntityFramework.Core.Entity
{
    public class UserChatSession
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public HimuHomeUser User { get; set; } = null!;
        public long FriendId { get; set; }
        public HimuHomeUser Friend { get; set; } = null!;
        public virtual ICollection<ChatMessage> Messages { get; set; } = null!;
    }
}
