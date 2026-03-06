using FluentValidation;
using SupportDesk.Api.Dtos.Auth;

namespace SupportDesk.Api.Validators.Auth;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    private static readonly string[] AllowedRoles = { "User", "Agent", "Admin" };

    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be valid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required")
            .Must(r => AllowedRoles.Contains(r))
            .WithMessage("Role must be User, Agent, or Admin");
    }
}