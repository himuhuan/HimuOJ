using Himu.EntityFramework.Core.Contests;
using Himu.EntityFramework.Core.Entity;
using Himu.EntityFramework.Core.Entity.Components;
using Himu.Home.HttpApi.Request;
using Himu.Home.HttpApi.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Himu.Home.HttpApi.Services.Context.Implement
{
    public class OJContextService : IOJContextService
    {
        private readonly HimuOnlineJudgeContext _ojContext;

        public OJContextService(HimuOnlineJudgeContext ojContext)
        {
            _ojContext = ojContext;
        }

        public async Task<HimuContest?> GetContest(long id)
        {
            return await _ojContext.Contests.FindAsync(id);
        }

        public async Task<HimuProblem?> GetProblem(long id)
        {
            return await _ojContext.ProblemSet.FindAsync(id);
        }

        public async Task<HimuProblem?> GetProblemWithContest(long id)
        {
            return await _ojContext.ProblemSet
                .Include(p => p.Contest)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<HimuCommit?> GetCommitById(long id)
        {
            return await _ojContext.UserCommits.FindAsync(id);
        }

        /// <summary>
        /// Add a commit to the context with test point results.
        /// </summary>
        /// <exception cref="ArgumentException"> The problem to commit don't have test point to test.  </exception>
        /// <returns> The commit added to the context. </returns>
        public async Task<HimuCommit> AddCommit(HimuCommit commit)
        {
            HimuProblem problem = commit.Problem;

            var testPoints = await _ojContext.TestPoints
                .Where(t => t.ProblemId == problem.Id)
                .ToListAsync();

            if (testPoints.Count == 0)
            {
                throw new ArgumentException("No test points found for the problem.");
            }

            using var transaction = _ojContext.Database.BeginTransaction();
            try
            {
                await _ojContext.UserCommits.AddAsync(commit);
                List<TestPointResult> results = testPoints.Select(t => new TestPointResult
                {
                    TestPointId = t.Id,
                    Commit = commit,
                    TestStatus = ExecutionStatus.PENDING
                }).ToList();
                await _ojContext.PointResults.AddRangeAsync(results);
                await transaction.CommitAsync();
                await _ojContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

            return commit;
        }

        /// <summary>
        /// Remove all commits of a user.
        /// </summary>
        /// TODO: solve Concurrency Check support
        public async Task RemoveUserAllCommits(long userId)
        {
            await _ojContext.UserCommits
                .Where(c => c.UserId == userId)
                .ExecuteDeleteAsync();
            await _ojContext.SaveChangesAsync();
        }

        public async Task RemoveCommit(long commitId)
        {
            HimuCommit? commit = await _ojContext.UserCommits.FindAsync(commitId)
                ?? throw new ArgumentException("Commit not found");
            _ojContext.UserCommits.Remove(commit);
            await _ojContext.SaveChangesAsync();
        }

        public async Task UpdateCommit(HimuCommit commit)
        {
            _ojContext.Update(commit);
            await _ojContext.SaveChangesAsync();
        }

        /// <summary>
        /// Get the commit count of a user.
        /// </summary>
        public async Task<long> GetUserCommitCount(long userId)
        {
            return await _ojContext.UserCommits
                .Where(c => c.UserId == userId)
                .LongCountAsync();
        }

        public async Task<HimuCommit?> GetFullCommit(long commitId)
        {
            return await _ojContext.UserCommits
                .AsNoTracking()
                .Include(c => c.Problem)
                .Include(c => c.TestPointResults)
                .Include(c => c.CompilerPreset)
                .FirstOrDefaultAsync(c => c.Id == commitId);
        }

        public async Task<List<HimuCommitListInfo>> GetCommitList(int page, int size,
                                                                      ListCommitFilterRequest? filter)
        {
            var query = _ojContext.UserCommits
                .AsNoTracking()
                .Include(c => c.Problem);
            IQueryable<HimuCommit>? res =
                (filter != null ? filter.ApplyFilter(query) : query) ?? throw new ArgumentException("Invalid filter");
            return await res.OrderByDescending(c => c.Id)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(c => new HimuCommitListInfo(c))
                .ToListAsync();
        }

        public async Task<long> GetTotalCommitCount(ListCommitFilterRequest? filter)
        {
            var query = _ojContext.UserCommits.AsNoTracking();
            IQueryable<HimuCommit>? res =
                filter != null ? filter.ApplyFilter(query) : query;

            if (res == null)
            {
                throw new ArgumentException("Invalid filter");
            }

            return await res.LongCountAsync();
        }

        public async Task AddProblem(HimuProblem problemToPost)
        {
            await _ojContext.ProblemSet.AddAsync(problemToPost);
            await _ojContext.SaveChangesAsync();
        }

        public async Task AddContest(HimuContest contestToAdd)
        {
            await _ojContext.Contests.AddAsync(contestToAdd);
            await _ojContext.SaveChangesAsync();
        }

        public async Task<long> GetContestIdFromCode(string code)
        {
            return await _ojContext.Contests.AsNoTracking()
                                            .Where(c => c.Information.Code == code)
                                            .Select(c => c.Id)
                                            .SingleOrDefaultAsync();
        }

        public async Task<long> GetProblemIdFromCode(string contestCode, string problemCode)
        {
            long problemId = await _ojContext.ProblemSet
                                          .AsNoTracking()
                                          .Include(p => p.Contest)
                                          .Where(p =>
                                              p.Detail.Code == problemCode &&
                                              p.Contest.Information.Code == contestCode)
                                          .Select(p => p.Id)
                                          .SingleOrDefaultAsync();
            return problemId;
        }

        public async Task<bool> IsProblemExists(long contestId, string problemCode)
        {
            return await _ojContext.ProblemSet
                .Where(p => p.ContestId == contestId && p.Detail.Code == problemCode)
                .AnyAsync();
        }

        public async Task<HimuProblemDetail?> GetProblemDetail(long id)
        {
            return await _ojContext.ProblemSet.AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => p.Detail)
                .SingleOrDefaultAsync();
        }

        public async Task<List<HimuProblemListInfo>>
            GetProblemList(int page, int size, ListProblemFilterRequest? filter)
        {
            if (page <= 0 || size <= 0)
            {
                throw new ArgumentException("Invalid page or size");
            }

            IQueryable<HimuProblem> query = _ojContext.ProblemSet.AsNoTracking();
            IQueryable<HimuProblem> filteredQuery = (filter?.ApplyFilter(query) ?? query)
                ?? throw new ArgumentException("Invalid filter");

            var problems = await filteredQuery
                .OrderBy(p => p.Id)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(p => new HimuProblemListInfo
                {
                    ProblemId = p.Id,
                    ProblemTitle = p.Detail.Title,
                    ProblemCode = p.Detail.Code,
                    ContestTitle = p.Contest.Information.Title,
                    ContestCode = p.Contest.Information.Code,
                })
                .ToListAsync();
            var problemIds = problems.Select(p => p.ProblemId).ToList();

            var commitCounts = await _ojContext.UserCommits
                .AsNoTracking()
                .Where(c => problemIds.Contains(c.ProblemId))
                .GroupBy(c => c.ProblemId)
                .Select(g => new
                {
                    ProblemId = g.Key,
                    AcceptedCount = g.Count(c => c.Status == ExecutionStatus.ACCEPTED),
                    TotalCommitCount = g.Count()
                })
                .ToDictionaryAsync(g => g.ProblemId);

            foreach (var problem in problems)
            {
                if (commitCounts.TryGetValue(problem.ProblemId, out var counts))
                {
                    problem.AcceptedCount = counts.AcceptedCount;
                    problem.TotalCommitCount = counts.TotalCommitCount;
                }
                else
                {
                    problem.AcceptedCount = 0;
                    problem.TotalCommitCount = 0;
                }
            }

            return problems;
        }

        public async Task<long> GetTotalProblemCount(ListProblemFilterRequest? filter)
        {
            var query = _ojContext.ProblemSet.AsNoTracking();
            IQueryable<HimuProblem>? res =
                filter != null ? filter.ApplyFilter(query) : query;

            if (res == null)
            {
                throw new ArgumentException("Invalid filter");
            }

            return await res.LongCountAsync();
        }

        public async Task<bool> IsProblemExists(long problemId)
        {
            return await _ojContext.ProblemSet
                .AsNoTracking()
                .AnyAsync(p => p.Id == problemId);
        }

        public async Task UpdateProblem(HimuProblem problemToUpdate)
        {
            _ojContext.Entry(problemToUpdate).State = EntityState.Modified;
            await _ojContext.SaveChangesAsync();
        }

        public async Task<HimuProblem?> GetProblemWithTestPoint(long problemId)
        {
            return await _ojContext.ProblemSet.AsNoTracking()
                .Include(p => p.TestPoints)
                .FirstOrDefaultAsync(p => p.Id == problemId);
        }

        public async Task AddTestPoint(HimuProblem problem, HimuTestPoint testPointToAdd)
        {
            _ojContext.Entry(problem).State = EntityState.Modified;
            problem.TestPoints.Add(testPointToAdd);
            await _ojContext.SaveChangesAsync();
        }

        public async Task<HimuTestPoint?> GetTestPointWithProblem(long testPointId)
        {
            return await _ojContext.TestPoints
                .AsNoTracking()
                .Include(t => t.Problem)
                .FirstOrDefaultAsync(t => t.Id == testPointId);
        }

        /// <summary>
        /// Get the contests that a user is authorized to access.
        /// A user is authorized to access a contest 
        /// if he is the distributor of the contest or he is the creator of the contest.
        /// </summary>
        public async Task<List<AuthorizedContestsInfo>> GetUserAuthorizedContests(long userId)
        {
            List<AuthorizedContestsInfo> contestsUserOwned = await _ojContext.Contests
                .AsNoTracking()
                .Where(c => c.DistributorId == userId)
                .Select(c => new AuthorizedContestsInfo(c.Id, c.Information.Title, c.Information.Code))
                .ToListAsync();
            List<AuthorizedContestsInfo> authorizedContests = await _ojContext.ContestCreators
                .AsNoTracking()
                .Where(cc => cc.CreatorId == userId)
                .Include(cc => cc.Contest)
                .Select(cc => new AuthorizedContestsInfo(
                    cc.ContestId, cc.Contest.Information.Title, cc.Contest.Information.Code))
                .ToListAsync();
            authorizedContests.AddRange(contestsUserOwned);
            return authorizedContests;
        }

        public async Task<CompilerPreset?> GetCompilerPreset(string name)
        {
            return await _ojContext.CompilerPresets.FindAsync(name);
        }

        public async Task<HimuCommit?> GetCommitForJudge(long commitId)
        {
            return await _ojContext.UserCommits
                .AsNoTracking()
                .Include(c => c.CompilerPreset)
                .Include(c => c.TestPointResults)
                .FirstOrDefaultAsync(c => c.Id == commitId);
        }
    }
}
