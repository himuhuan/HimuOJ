using Himu.EntityFramework.Core.Entity;
using Himu.Home.HttpApi.Request;
using Himu.Home.HttpApi.Response;

namespace Himu.Home.HttpApi.Services.Context
{
    public interface IOJContextService
    {
        // 
        // Commits
        //

        Task<HimuCommit> AddCommit(HimuCommit commit);
        Task<HimuCommit?> GetCommitById(long id);
        Task<List<HimuCommitListInfo>> GetCommitList(int page, int size, ListCommitFilterRequest? filter);
        Task<HimuCommit?> GetFullCommit(long commitId);
        Task<HimuCommit?> GetCommitForJudge(long commitId);
        Task<long> GetTotalCommitCount(ListCommitFilterRequest? filter);
        Task<long> GetUserCommitCount(long userId);
        Task RemoveUserAllCommits(long userId);

        //
        // Problems
        //

        Task<HimuProblem?> GetProblem(long id);
        Task<HimuProblemDetail?> GetProblemDetail(long id);
        Task<HimuProblem?> GetProblemWithContest(long id);
        Task AddProblem(HimuProblem problemToAdd);
        Task<long> GetProblemIdFromCode(string contestCode, string problemCode);
        Task<HimuProblem?> GetProblemWithTestPoint(long problemId);
        Task<bool> IsProblemExists(long contestId, string problemCode);
        Task<long> GetTotalProblemCount(ListProblemFilterRequest? filter);
        Task<List<HimuProblemListInfo>> GetProblemList(int page, int size, ListProblemFilterRequest? filter);
        Task<bool> IsProblemExists(long problemId);
        Task UpdateProblem(HimuProblem problemToUpdate);

        //
        // Contests
        //

        Task<HimuContest?> GetContest(long id);
        Task AddContest(HimuContest contestToAdd);
        Task<long> GetContestIdFromCode(string code);
        Task<List<AuthorizedContestsInfo>> GetUserAuthorizedContests(long userId);

        //
        // Test Points
        //

        /// <summary>
        /// Add a test point to a problem. 
        /// </summary>
        /// <param name="problem"> Problem must from <seealso cref="GetProblemWithTestPoint(long)"/> </param>
        /// <remarks>
        /// After adding a test point, the problem will be <b>tracked</b> by the context and be updated.
        /// </remarks>
        Task AddTestPoint(HimuProblem problem, HimuTestPoint testPointToAdd);
        /// <summary>
        /// Get a test point with problem.
        /// </summary>
        Task<HimuTestPoint?> GetTestPointWithProblem(long testPointId);

        //
        // Compiler Preset
        //

        Task<CompilerPreset?> GetCompilerPreset(string name);
        Task RemoveCommit(long commitId);
        Task UpdateCommit(HimuCommit commit);
    }
}