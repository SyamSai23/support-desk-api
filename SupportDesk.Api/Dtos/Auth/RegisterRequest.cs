namespace SupportDesk.Api.Dtos.Auth;

public class RegisterRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string Role { get; set; } = "User"; // optional, keep User by default
}