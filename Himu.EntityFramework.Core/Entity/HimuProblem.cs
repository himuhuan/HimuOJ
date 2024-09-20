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

        public long ContestId { get; set; }

        [JsonIgnore]
        public HimuContest Contest { get; set; } = null!;

        [JsonIgnore]
        public ICollection<HimuTestPoint> TestPoints { get; set; } = null!;

        [JsonIgnore]
        public ICollection<HimuCommit> UserCommits { get; set; } = null!;
    }
}