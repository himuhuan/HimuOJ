namespace Himu.HttpApi.Utility
{
    public record UserRegisterRequest(
        string UserName, string Password,
        string RepeatedPassword, string Mail, string PhoneNumber);
}