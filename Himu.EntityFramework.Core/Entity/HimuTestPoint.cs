namespace Himu.EntityFramework.Core.Entity
{
    /// <summary>
    /// A TestPoint for a problem, Input, Output, Expected can be file or user-defined data
    /// </summary>
    public class HimuTestPoint
    {
        public long Id { get; set; }

        public string CaseName { get; set; } = null!;

        public string Input { get; set; } = string.Empty;

        public string Expected { get; set; } = string.Empty;

        public HimuProblem Problem { get; set; } = null!;

        public long ProblemId { get; set; }
    }
}