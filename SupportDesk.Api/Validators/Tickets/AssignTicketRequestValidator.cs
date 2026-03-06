using FluentValidation;
using SupportDesk.Api.Dtos.Tickets;

namespace SupportDesk.Api.Validators.Tickets;

public class AssignTicketRequestValidator : AbstractValidator<AssignTicketRequest>
{
    public AssignTicketRequestValidator()
    {
        RuleFor(x => x.AssignedToUserId)
            .GreaterThan(0).WithMessage("AssignedToUserId must be greater than 0");
    }
}