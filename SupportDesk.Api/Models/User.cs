namespace SupportDesk.Api.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "User"; // User / Agent / Admin
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}