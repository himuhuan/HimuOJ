using Himu.EntityFramework.Core.Entity;

namespace Himu.Home.HttpApi.Request
{
    /// <summary>
    /// Filter for listing problems
    /// </summary>
    public class ListProblemFilterRequest
    {
        public long? ContestId { get; init; } = null;

        /// <summary>
        /// Contest code: when ContestId is not null, this field is ignored
        /// </summary>
        public string? ContestCode { get; init; } = null;

        /// <summary>
        /// The user who created the contest which the problem belongs to
        /// </summary>
        public long? AdministratorId { get; init; } = null;

        /// <summary>
        /// The user who created the problem
        /// </summary>
        public long? CreatorId { get; init; } = null;

        /// <summary>
        /// The user who authenticated to modify the problem
        /// </summary>
        public long? AuthenticatedUserId { get; init; } = null;

        public IQueryable<T>? ApplyFilter<T>(IQueryable<T> query) where T : HimuProblem
        {
            if (!string.IsNullOrEmpty(ContestCode) && ContestId == null)
            {
                query = query.Where(p => p.Contest.Information.Code == ContestCode);
            }

            if (ContestId != null)
            {
                query = query.Where(p => p.ContestId == ContestId);
            }

            // For now, we assume that Administrator, Creator, and Authenticated user
            // are the same user.
            // TODO: Implement the logic for different users
            if (AdministratorId != null)
            {
                query = query.Where(p => p.Contest.DistributorId == AdministratorId);
            }
            if (CreatorId != null)
            {
                query = query.Where(p => p.DistributorId == CreatorId);
            }
            if (AuthenticatedUserId != null)
            {
                query = query.Where(p => p.DistributorId == AuthenticatedUserId);
            }
            return query;
        }
    }
}