namespace Himu.Home.HttpApi.Request
{
    public record ResetUserPasswordRequest(string Mail, string Token, string NewPassword);
}