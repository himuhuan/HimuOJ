using FluentValidation;
using Himu.HttpApi.Utility.Request;

namespace Himu.HttpApi.Utility.Validation
{
    public class CheckCurdPermissionRequestValidator : AbstractValidator<CheckCurdPermissionRequest>
    {
        public CheckCurdPermissionRequestValidator()
        {
            RuleFor(request => request.UserId)
                .GreaterThan(0)
                .WithMessage("UserId must be greater than 0");

            RuleFor(request => request.EntityType)
                .NotEmpty()
                .WithMessage("EntityType must not be empty");

            RuleFor(request => request.EntityId)
                .GreaterThan(0)
                .WithMessage("EntityId must be greater than 0");

            RuleFor(Request => Request.EntityType)
                .Must(entityType => entityType == "problem" || entityType == "contest")
                .WithMessage("EntityType must be either 'problem' or 'contest'");
        }
    }
}
