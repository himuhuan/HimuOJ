using FluentValidation;

namespace Himu.HttpApi.Utility
{
    public class UserRegisterRequestValidator : AbstractValidator<UserRegisterRequest>
    {
        public UserRegisterRequestValidator()
        {
            RuleFor(u => u.UserName).NotEmpty().Length(3, 50);
            RuleFor(u => u.Password)
                .Length(6, 50)
                .Equal(u => u.RepeatedPassword);
            RuleFor(u => u.Mail).NotEmpty().EmailAddress();
        }
    }
}