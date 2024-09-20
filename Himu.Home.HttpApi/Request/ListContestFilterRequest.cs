namespace Himu.Home.HttpApi.Request;

using EntityFramework.Core.Entity;

/// <summary>
/// Filter for listing contests
/// </summary>
public class ListContestFilterRequest
{
    /// <summary>
    /// Contest code for filtering
    /// </summary>
    public string? ContestCode { get; init; } = null;

    /// <summary>
    /// Contest title for filtering
    /// </summary>
    public string? ContestTitle { get; init; } = null;

    /// <summary>
    /// The user who created the contest
    /// </summary>
    public long? CreatorId { get; init; } = null;

    public IQueryable<HimuContest>? ApplyFilter(IQueryable<HimuContest> query)
    {
        if (!string.IsNullOrEmpty(ContestCode))
        {
            query = query.Where(c => c.Information.Code == ContestCode);
        }

        if (!string.IsNullOrEmpty(ContestTitle))
        {
            query = query.Where(c => c.Information.Title.Contains(ContestTitle));
        }

        if (CreatorId != null)
        {
            query = query.Where(c => c.DistributorId == CreatorId);
        }

        return query;
    }
}