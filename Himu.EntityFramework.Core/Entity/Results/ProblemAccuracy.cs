using System.ComponentModel.DataAnnotations;

namespace Himu.EntityFramework.Core.Entity.Results;


public class ProblemAccuracy
{
    [Key]
    public long ProblemId { get; set; }

    public long ProblemAcceptedCount { get; set; }
    public long ProblemCommitCount { get; set; }
    public double AccuracyRate { get; set; }
}