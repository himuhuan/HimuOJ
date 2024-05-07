using FluentValidation;
using Himu.EntityFramework.Core.Entity;

namespace Himu.HttpApi.Utility
{
    public class HimuProblemDetailVaildator : AbstractValidator<HimuProblemDetail>
    {
        public HimuProblemDetailVaildator()
        {
            RuleFor(info => info.Code)
                .NotEmpty()
                .Matches("^[a-z0-9-]+$")
                .WithMessage("访问代码必须由小写字母或数字与连字符组成");
        }
    }
}