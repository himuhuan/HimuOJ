namespace Himu.HttpApi.Utility
{
    public record VerifyEmailConfirmationRequest(string UserName, string ConfirmationToken);
}