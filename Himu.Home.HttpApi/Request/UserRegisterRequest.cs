namespace Himu.Home.HttpApi.Request
{
    public record UserRegisterRequest(
        string UserName, string Password,
        string RepeatedPassword, string Mail, string PhoneNumber);
}