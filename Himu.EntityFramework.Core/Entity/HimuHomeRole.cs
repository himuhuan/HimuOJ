using Microsoft.AspNetCore.Identity;

namespace Himu.EntityFramework.Core.Entity
{
    public class HimuHomeRole : IdentityRole<long>
    {
        public const string StandardUser = "StandardUser";
        public const string Administrator = "Administrator";

        /// <summary>
        /// Has the right to publish problems-
        /// </summary>
        public const string Publisher = "ProblemPublisher";

        /// <summary>
        /// A dictionary that stores the priority of each role.
        /// The lower the value, the higher the permission.
        /// </summary>
        public static readonly Dictionary<string, int> RolePermissionPriority = new()
        {
            { Administrator,    0},
            { Publisher,        1},
            { StandardUser,     2},
        };

        public static string GetHighestRole(IEnumerable<string> roles)
        {
            PriorityQueue<string, int> queue = new();
            foreach (var role in roles)
            {
                if (RolePermissionPriority.ContainsKey(role))
                {
                    queue.Enqueue(role, RolePermissionPriority[role]);
                }
            }
            return queue.Count > 0 ? queue.Peek() : StandardUser;
        }
    }
}