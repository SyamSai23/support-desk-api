using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportDesk.Api.Dtos.Auth;
using SupportDesk.Api.Services.Auth;

namespace SupportDesk.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    // Public registration: always creates User
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        var ok = await _auth.RegisterAsync(req);
        if (!ok) return BadRequest(new { error = "Email already exists" });
        return Ok(new { message = "Registered successfully" });
    }

    // Admin-only: can create Agent/Admin/User
    [Authorize(Roles = "Admin")]
    [HttpPost("create-user")]
    public async Task<IActionResult> CreateUser(CreateUserRequest req)
    {
        var ok = await _auth.CreateUserAsync(req);
        if (!ok) return BadRequest(new { error = "Email already exists" });
        return Ok(new { message = "User created successfully" });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest req)
    {
        var res = await _auth.LoginAsync(req);
        if (res is null) return Unauthorized(new { error = "Invalid email or password" });
        return Ok(res);
    }
}