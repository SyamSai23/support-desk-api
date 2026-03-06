using FluentValidation;
using SupportDesk.Api.Dtos.Tickets;

namespace SupportDesk.Api.Validators.Tickets;

public class UpdateTicketRequestValidator : AbstractValidator<UpdateTicketRequest>
{
    private static readonly string[] AllowedStatuses = { "Open", "InProgress", "Closed" };

    public UpdateTicketRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title must be <= 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description must be <= 2000 characters");

        RuleFor(x => x.Status)
            .Must(s => string.IsNullOrWhiteSpace(s) || AllowedStatuses.Contains(s))
            .WithMessage("Status must be Open, InProgress, or Closed");
            }
}