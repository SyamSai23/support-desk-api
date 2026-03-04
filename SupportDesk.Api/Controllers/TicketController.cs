using Microsoft.AspNetCore.Mvc;
using SupportDesk.Api.Dtos.Tickets;
using SupportDesk.Api.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SupportDesk.Api.Dtos.Tickets.Comments;
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

    // GET /tickets?page=1&pageSize=10&status=Open&search=login
    [HttpGet]
    public async Task<ActionResult<PagedResult<TicketResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null,
        [FromQuery] string? search = null)
    {
        var result = await _service.GetAllPagedAsync(page, pageSize, status, search);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TicketResponse>> GetById(int id)
    {
        var ticket = await _service.GetByIdAsync(id);
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
        var updated = await _service.UpdateAsync(id, req);
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
        var items = await _comments.GetCommentsAsync(id);
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