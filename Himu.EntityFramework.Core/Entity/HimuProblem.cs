using System.Text.Json.Serialization;

namespace Himu.EntityFramework.Core.Entity
{
    
    public class HimuProblem
    {
        public long Id;

        public long DistributorId { get; set; }

        [JsonIgnore]
        public HimuHomeUser Distributor { get; set; } = null!;

        public HimuProblemDetail Detail { get; set; } = null!;

        // Set via trigger
        public long ProblemCommitCount { get; set; }

        // Set via trigger
        public long ProblemAcceptedCount { get; set; }

        public long ContestId { get; set; }

        [JsonIgnore]
        public HimuContest Contest { get; set; } = null!;

        [JsonIgnore]
        public ICollection<HimuTestPoint> TestPoints { get; set; } = null!;

        [JsonIgnore]
        public ICollection<HimuCommit> UserCommits { get; set; } = null!;
    }
}