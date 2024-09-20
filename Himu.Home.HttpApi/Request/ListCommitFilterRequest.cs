using Himu.EntityFramework.Core.Entity;

namespace Himu.Home.HttpApi.Request;

/// <summary>
/// Dto for filtering the list of commits.
/// </summary>
public class ListCommitFilterRequest
{
    public long? ProblemId { get; init; } = null;

    public long? UserId { get; init; } = null;

    public string? ProblemName { get; init; } = null;

    public string? CommitStatus { get; init; } = null;

    public string? Language { get; init; } = null;

    public long? CommitDateEnd { get; init; } = null;

    public IQueryable<T>? ApplyFilter<T>(IQueryable<T> query) where T : HimuCommit
    {
        // filter

        if (ProblemId != null)
        {
            query = query.Where(c => c.ProblemId == ProblemId);
        }

        if (UserId != null)
        {
            query = query.Where(c => c.UserId == UserId);
        }

        // If it has problem id, ignore problem name
        if (ProblemId == null && !string.IsNullOrEmpty(ProblemName))
        {
            query = query.Where(c => c.Problem.Detail.Title.Contains(ProblemName));
        }

        if (!string.IsNullOrEmpty(Language))
        {
            query = query.Where(c => c.CompilerName == Language);
        }

        if (!string.IsNullOrEmpty(CommitStatus))
        {
            if (Enum.TryParse(CommitStatus, out ExecutionStatus status))
                query = query.Where(c => c.Status == status);
            else
                return null;
        }

        if (CommitDateEnd != null)
        {
            DateOnly date = DateOnly.FromDateTime(DateTimeOffset
                                                  .FromUnixTimeMilliseconds(CommitDateEnd.Value)
                                                  .LocalDateTime);
            query = query.Where(c => c.CommitDate <= date);
        }

        return query;
    }
}