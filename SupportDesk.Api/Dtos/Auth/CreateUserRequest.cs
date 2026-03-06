namespace SupportDesk.Api.Dtos.Auth;

public class CreateUserRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string Role { get; set; } = "User";
}