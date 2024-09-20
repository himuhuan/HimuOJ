namespace Himu.Home.HttpApi.Request
{
    public record CommitSourceCodeRequest(long ProblemId, string SourceCode, string CompilerPresetName);
}