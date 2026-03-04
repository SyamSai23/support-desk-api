namespace SupportDesk.Api.Dtos.Tickets.Comments;

public class CommentResponse
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public int UserId { get; set; }
    public string UserEmail { get; set; } = "";
    public string Message { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}