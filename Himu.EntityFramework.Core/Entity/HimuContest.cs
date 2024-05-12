using Himu.EntityFramework.Core.Entity.Components;

namespace Himu.EntityFramework.Core.Entity
{
    public class HimuContest
    {
        public long Id { get; set; }

        //< Migration@2024-05-10: Remove DistributeDateTime and add CreateDate to HimuContest
        public DateOnly CreateDate { get; set; }

        public HimuContestInformation Information { get; set; } = null!;

        public long DistributorId { get; set; }

        /// <summary>
        /// Authors of this contest
        /// </summary>
        public HimuHomeUser Distributor { get; set; } = null!;

        public ICollection<HimuProblem>? Problems { get; set; }

        /// <summary>
        /// Not included the distributor himself
        /// </summary>
        public ICollection<HimuHomeUser>? Creators { get; set; }
    }
}