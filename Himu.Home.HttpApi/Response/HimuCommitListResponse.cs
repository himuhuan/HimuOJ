using Himu.EntityFramework.Core.Entity;

namespace Himu.Home.HttpApi.Response
{
    public class HimuCommitListInfo
    {
        public long CommitId { get; set; }

        public long UserId { get; set; }

        public long ProblemId { get; set; }

        public string ProblemName { get; set; } = string.Empty;

        public ExecutionStatus CommitStatus { get; set; }

        public string CompilerName { get; set; } = string.Empty;

        public DateOnly CommitDate { get; set; }
         
        public HimuCommitListInfo()
        {
        }

        public HimuCommitListInfo(long commitId, long userId, long problemId, string problemName,
                                  ExecutionStatus commitStatus, string compilerName, DateOnly commitDate)
        {
            CommitId = commitId;
            UserId = userId;
            ProblemId = problemId;
            ProblemName = problemName;
            CommitStatus = commitStatus;
            CompilerName = compilerName;
            CommitDate = commitDate;
        }

        public HimuCommitListInfo(HimuCommit commit)
        {
            CommitId = commit.Id;
            UserId = commit.UserId;
            ProblemId = commit.ProblemId;
            ProblemName = commit.Problem.Detail.Title;
            CommitStatus = commit.Status;
            CompilerName = commit.CompilerName;
            CommitDate = commit.CommitDate;
        }
    }

    public class HimuCommitListResponseValue
    {
        public List<HimuCommitListInfo> Data { get; set; } = null!;
        public long TotalCount { get; set; }
        public int PageCount { get; set; }
    }

    public class HimuCommitListResponse : HimuApiResponse<HimuCommitListResponseValue>
    {
        public HimuCommitListResponse Success(
            List<HimuCommitListInfo> pagedCommits,
            long total,
            int pageSize
        )
        {
            Value = new HimuCommitListResponseValue
            {
                Data = pagedCommits,
                PageCount = (int) Math.Ceiling(total * 1.0 / pageSize),
                TotalCount = total
            };
            return this;
        }
    }
}