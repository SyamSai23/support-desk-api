using FluentValidation;
using SupportDesk.Api.Dtos.Tickets.Comments;

namespace SupportDesk.Api.Validators.Tickets.Comments;

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required")
            .MaximumLength(1000).WithMessage("Message must be <= 1000 characters");
    }
}