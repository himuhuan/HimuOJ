using Himu.EntityFramework.Core.Entity;
using Himu.HttpApi.Utility.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Himu.HttpApi.Utility.Request
{
    public class CheckCurdPermissionRequest
    {
        public long UserId { get; init; }

        /// <summary>
        /// Entity name to check: can be the following values which require checking CURD permissions:
        /// <list type="bullet"> 
        /// <item> problem: <see cref="HimuProblem"/> </item>
        /// <item> contest: <see cref="HimuContest"/> </item>
        /// </list>
        /// </summary>
        public string EntityType { get; init; } = null!;

        public long EntityId { get; init; }

        public OperationAuthorizationRequirement Operation { get; init; } = HimuCrudOperations.Read;
    }
}
