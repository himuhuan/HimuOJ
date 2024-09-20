using Himu.EntityFramework.Core.Entity;
using Himu.EntityFramework.Core.Entity.Components;

namespace Himu.Home.HttpApi.Response
{
    public record HimuProblemSearchResult(string UserName, HimuContestInformation Contest, HimuProblemDetail Detail);
}