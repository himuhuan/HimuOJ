namespace Himu.Home.HttpApi.Request
{
    public record UserLoginRequest(string Input, string Method, string Password);

    public enum UserLoginMethod
    {
        User, Mail
    }
}