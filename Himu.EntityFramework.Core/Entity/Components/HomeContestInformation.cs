using Microsoft.EntityFrameworkCore;

namespace Himu.EntityFramework.Core.Entity.Components
{
    // [Owned] for Resharper warnings
    [Owned]
    public class HimuContestInformation
    {
        public string Code { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Introduction { get; set; } = string.Empty;

        //< Migration@2024-05-10: Remove DistributeDateTime and add CreateDate to HimuContest
        // public DateTime DistributeDateTime { get; set; } = DateTime.Now;

        public bool LaunchTaskAtOnce { get; set; } = false;
    }
}