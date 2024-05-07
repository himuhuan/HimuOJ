using Himu.EntityFramework.Core.Entity;
using Himu.EntityFramework.Core.Entity.Components;

namespace Himu.HttpApi.Utility
{
    public record HimuProblemSearchResult(string UserName, HimuContestInformation Contest, HimuProblemDetail Detail);
}