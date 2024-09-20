using Himu.EntityFramework.Core.Entity.Components;
using Himu.EntityFramework.Core.Tools;
using System.Text.Json.Serialization;

namespace Himu.EntityFramework.Core.Entity
{
    public class TestPointResult
    {
        public long Id { get; set; }

        public ExecutionStatus TestStatus { get; set; }

        [JsonConverter(typeof(ResourceUsageJsonConverter))]
        public ResourceUsage? Usage { get; set; }

        public OutputDifference? Difference { get; set; }

        public long TestPointId { get; set; }

        public long CommitId { get; set; }

        /// <summary>
        /// One-way navigation to HimuTestPoint
        /// </summary>
        [JsonIgnore]
        public HimuTestPoint TestPoint { get; set; } = null!;

        /// <summary>
        /// One-to-many
        /// </summary>
        [JsonIgnore]
        public HimuCommit Commit { get; set; } = null!;
    }
}