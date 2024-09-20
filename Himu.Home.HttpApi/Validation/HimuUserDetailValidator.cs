using FluentValidation;
using Himu.Home.HttpApi.Response;

namespace Himu.Home.HttpApi.Validation
{
    public class HimuUserDetailValidator : AbstractValidator<HimuUserDetail>
    {
        public HimuUserDetailValidator()
        {
            RuleFor(u => u.UserName).NotEmpty().Length(3, 50);
            RuleFor(u => u.Email).NotEmpty().EmailAddress();
            RuleFor(u => u.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\d{8,15}$").WithMessage("Phone number must be between 8 and 15 digits.");
        }
    }
}
