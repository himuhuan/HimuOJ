namespace Himu.HttpApi.Utility
{
    /// <summary>
    /// The enumeration value of the <see cref="HimuActionCheckAttribute"/> attribute.
    /// </summary>
    public enum HimuActionCheckTargets
    {
        None = 0,

        /// <summary>
        /// Check the user is the administrator.
        /// </summary>
        Administrator,

        /// <summary>
        /// Check the user is the distributor of the contest.
        /// </summary>
        ContestDistributor,

        /// <summary>
        /// Check the user has the permission to modify the problem.
        /// </summary>
        ProblemModify,
    }
}
