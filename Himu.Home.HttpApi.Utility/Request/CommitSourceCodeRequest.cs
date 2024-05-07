namespace Himu.HttpApi.Utility.Request
{
    public record CommitSourceCodeRequest(long ProblemId, string SourceCode, string Language);
}
