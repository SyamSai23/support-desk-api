using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportDesk.Api.Dtos.Tickets;
using SupportDesk.Api.Dtos.Tickets.Comments;
using SupportDesk.Api.Services;

namespace SupportDesk.Api.Controllers;

[Authorize]
[ApiController]
[Route("tickets")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _service;
    private readonly ITicketCommentService _comments;

    public TicketsController(ITicketService service, ITicketCommentService comments)
    {
        _service = service;
        _comments = comments;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<TicketResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null,
        [FromQuery] string? search = null)
    {
        var userIdStr = User.FindFirstValue("uid");
        var role = User.FindFirstValue(ClaimTypes.Role) ?? "User";

        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

        var userId = int.Parse(userIdStr);

        var result = await _service.GetAllPagedAsync(page, pageSize, status, search, userId, role);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TicketResponse>> GetById(int id)
    {
        var userIdStr = User.FindFirstValue("uid");
        var role = User.FindFirstValue(ClaimTypes.Role) ?? "User";

        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

        var userId = int.Parse(userIdStr);

        var ticket = await _service.GetByIdAsync(id, userId, role);
        if (ticket is null) return NotFound(new { error = $"Ticket {id} not found" });

        return Ok(ticket);
    }

    [HttpPost]
    public async Task<ActionResult<TicketResponse>> Create(CreateTicketRequest req)
    {
        var userIdStr = User.FindFirstValue("uid");
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

        var userId = int.Parse(userIdStr);

        var created = await _service.CreateAsync(req, userId);
        return Created($"/tickets/{created.Id}", created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TicketResponse>> Update(int id, UpdateTicketRequest req)
    {
        var role = User.FindFirstValue(ClaimTypes.Role) ?? "User";

        var updated = await _service.UpdateAsync(id, req, role);
        if (updated is null) return NotFound(new { error = $"Ticket {id} not found" });

        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(new { error = $"Ticket {id} not found" });

        return NoContent();
    }

    [HttpGet("{id:int}/comments")]
    public async Task<ActionResult<List<CommentResponse>>> GetComments(int id)
    {
        var userIdStr = User.FindFirstValue("uid");
        var role = User.FindFirstValue(ClaimTypes.Role) ?? "User";

        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

        var userId = int.Parse(userIdStr);

        var items = await _comments.GetCommentsAsync(id, userId, role);
        if (items is null) return NotFound(new { error = $"Ticket {id} not found" });

        return Ok(items);
    }

    [HttpPost("{id:int}/comments")]
    public async Task<ActionResult<CommentResponse>> AddComment(int id, CreateCommentRequest req)
    {
        var userIdStr = User.FindFirstValue("uid");
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

        var userId = int.Parse(userIdStr);

        var created = await _comments.AddCommentAsync(id, userId, req.Message);
        if (created is null) return NotFound(new { error = $"Ticket {id} not found" });

        return Created($"/tickets/{id}/comments/{created.Id}", created);
    }
}