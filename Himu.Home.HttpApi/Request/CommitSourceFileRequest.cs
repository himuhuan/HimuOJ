namespace Himu.Home.HttpApi.Request
{
    public record CommitSourceFileRequest(string ContestCode, string ProblemCode, IFormFile SourceFile);
}