using FluentValidation;
using Himu.Home.HttpApi.Request;

namespace Himu.Home.HttpApi.Validation
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
            RuleFor(u => u.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\d{8,15}$").WithMessage("Phone number must be between 8 and 15 digits.");
        }
    }
}