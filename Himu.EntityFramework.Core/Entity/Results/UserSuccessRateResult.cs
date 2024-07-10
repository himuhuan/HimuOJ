using Microsoft.EntityFrameworkCore;

namespace Himu.EntityFramework.Core.Entity.Results;

[Keyless]
public class UserSuccessRateResult
{
    public double SuccessRate { get; set; }
    public int TotalSubmits { get; set; }
    public int ProblemSolved { get; set; }
    public int SuccessfulSubmits { get; set; }
}