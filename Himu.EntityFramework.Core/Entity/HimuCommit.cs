using Himu.EntityFramework.Core.Entity.Components;
using System.Text.Json.Serialization;

namespace Himu.EntityFramework.Core.Entity
{
    public class HimuCommit
    {
        public long Id { get; set; }

        public string SourceUri { get; set; } = string.Empty;

        //> Mi2024-03-15 Add_HimuCommit_CommitDate
        //> Add CommitDate to HimuCommit
        public DateOnly CommitDate { get; set; }

        /// <summary>
        /// Status depends on the results of the individual test points
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExecutionStatus Status { get; set; } = ExecutionStatus.PENDING;

        public string CompilerName { get; set; } = string.Empty;

        public string MessageFromCompiler { get; set; } = string.Empty;

        /// <summary>
        /// Compiler preset used to compile the source code
        /// </summary>
        public CompilerPreset CompilerPreset { get; set; } = null!;

        public long UserId { get; set; }

        [JsonIgnore]
        public HimuHomeUser User { get; set; } = null!;

        public long ProblemId { get; set; }

        [JsonIgnore]
        public HimuProblem Problem { get; set; } = null!;

        public List<TestPointResult>? TestPointResults { get; set; }
    }
}