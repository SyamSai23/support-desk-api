namespace SupportDesk.Api.Dtos.Tickets;

public class TicketResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Status { get; set; } = "";
    public DateTime CreatedAt { get; set; }

    public int? CreatedByUserId { get; set; }
    public int? AssignedToUserId { get; set; }
    public string? AssignedToUserEmail { get; set; }
}