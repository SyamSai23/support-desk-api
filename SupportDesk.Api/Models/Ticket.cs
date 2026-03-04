namespace SupportDesk.Api.Models;

public class Ticket
{
    public int Id { get; set; }                  // primary key
    public string Title { get; set; } = "";      // required
    public string Description { get; set; } = ""; // required
    public string Status { get; set; } = "Open";  // Open / InProgress / Closed
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }
}