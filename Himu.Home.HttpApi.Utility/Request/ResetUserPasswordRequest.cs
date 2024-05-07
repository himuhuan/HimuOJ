namespace Himu.HttpApi.Utility
{
    public record ResetUserPasswordRequest(string Mail, string Token, string NewPassword);
}