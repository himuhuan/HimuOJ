namespace Himu.EntityFramework.Core.Entity
{
    public class ContestProblemCode
    {
        public long ProblemId { get; set; }
        public long ContestId { get; set; }

        public string ProblemCode { get; set; } = null!;
    }
}