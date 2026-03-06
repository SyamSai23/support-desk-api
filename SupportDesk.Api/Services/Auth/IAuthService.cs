using SupportDesk.Api.Dtos.Auth;

namespace SupportDesk.Api.Services.Auth;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterRequest req);

    Task<bool> CreateUserAsync(CreateUserRequest req);
    Task<AuthResponse?> LoginAsync(LoginRequest req);
}