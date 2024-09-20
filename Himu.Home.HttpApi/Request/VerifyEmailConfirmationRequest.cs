namespace Himu.Home.HttpApi.Request
{
    public record VerifyEmailConfirmationRequest(string UserName, string ConfirmationToken);
}