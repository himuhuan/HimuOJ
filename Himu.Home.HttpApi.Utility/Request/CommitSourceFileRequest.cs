using Microsoft.AspNetCore.Http;

namespace Himu.HttpApi.Utility.Request
{
    public record CommitSourceFileRequest(string ContestCode, string ProblemCode, IFormFile SourceFile);
}
