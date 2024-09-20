namespace Himu.EntityFramework.Core.Entity
{
    /// <summary>
    /// ContestCreator is a join entity between HimuContest and HimuHomeUser
    /// </summary>
    public class ContestCreator
    {
        public long ContestId { get; set; }
        public long CreatorId { get; set; }

        public HimuContest Contest { get; set; } = null!;
        public HimuHomeUser Creator { get; set; } = null!;
    }
}