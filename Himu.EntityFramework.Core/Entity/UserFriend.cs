namespace Himu.EntityFramework.Core.Entity
{
    public class UserFriend
    {
        public long UserId { get; set; }
        public long FriendId { get; set; }

        public HimuHomeUser User { get; set; } = null!;
        public HimuHomeUser Friend { get; set; } = null!;
    }
}
