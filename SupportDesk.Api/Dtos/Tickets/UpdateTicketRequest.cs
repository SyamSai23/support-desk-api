namespace SupportDesk.Api.Dtos.Tickets;

public class UpdateTicketRequest
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Status { get; set; } = "Open";
}