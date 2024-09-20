using FluentValidation;
using Himu.EntityFramework.Core.Entity.Components;

namespace Himu.Home.HttpApi.Validation
{
    public class HimuContestInformationValidator : AbstractValidator<HimuContestInformation>
    {
        public HimuContestInformationValidator()
        {
            RuleFor(info => info.Code)
                .NotEmpty()
                .Matches("^[a-z0-9-]+$")
                .WithMessage("访问代码必须由小写字母或数字与连字符组成");
        }
    }
}