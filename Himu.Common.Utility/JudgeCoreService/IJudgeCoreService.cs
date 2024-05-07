namespace Himu.Common.Service
{
    public interface IJudgeCoreService
    {
        /// <summary>
        /// send a request to judge core server to judge a commit
        /// </summary>
        /// <param name="commitId"> commit id to judge </param>
        /// <returns> a status code that server responsed </returns>
        Task<int> RequestJudgeCommitAsync(long commitId);
    }
}
