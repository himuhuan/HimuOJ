using Himu.EntityFramework.Core.Entity;

namespace Himu.Home.HttpApi.Services.Judge
{
    public interface IJudgeTaskDispatcher
    {
        Task DispatchCommitTaskAsync(HimuCommit commit, HimuProblem targetProblem, string connectionId);
    }
}
