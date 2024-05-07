namespace Himu.HttpApi.Utility
{
    public record UserLoginRequest(string Input, string Method, string Password);

    public enum UserLoginMethod
    {
        User, Mail
    }
}