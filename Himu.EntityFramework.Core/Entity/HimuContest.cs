using Himu.EntityFramework.Core.Entity.Components;

namespace Himu.EntityFramework.Core.Entity
{
    public class HimuContest
    {
        public long Id { get; set; }

        public HimuContestInformation Information { get; set; } = null!;

        public long DistributorId { get; set; }

        /// <summary>
        /// Authors of this contest
        /// </summary>
        public HimuHomeUser Distributor { get; set; } = null!;

        public ICollection<HimuProblem>? Problems { get; set; }

        // for EFCore
        // ReSharper disable once UnusedMember.Local
        private HimuContest()
        { }

        public HimuContest(string code, string title, string description, string introduction, DateTime distributeTime)
        {
            Information = new HimuContestInformation
            {
                Code = code,
                Title = title,
                Description = description,
                Introduction = introduction,
                DistributeDateTime = distributeTime
            };
        }

        public HimuContest(string code, string title, string description, string introduction)
            : this(code, title, description, introduction, DateTime.Now)
        {
        }
    }
}