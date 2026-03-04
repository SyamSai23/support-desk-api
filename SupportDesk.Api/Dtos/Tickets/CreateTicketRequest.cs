namespace SupportDesk.Api.Dtos.Tickets;

public class CreateTicketRequest
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
}