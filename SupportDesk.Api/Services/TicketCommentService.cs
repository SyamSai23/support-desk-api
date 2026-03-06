using Microsoft.EntityFrameworkCore;
using SupportDesk.Api.Data;
using SupportDesk.Api.Dtos.Tickets.Comments;
using SupportDesk.Api.Models;

namespace SupportDesk.Api.Services;

public class TicketCommentService : ITicketCommentService
{
    private readonly AppDbContext _db;

    public TicketCommentService(AppDbContext db) => _db = db;

    public async Task<CommentResponse?> AddCommentAsync(int ticketId, int callerUserId, string callerRole, string message)
    {
        var isAgentOrAdmin = callerRole == "Agent" || callerRole == "Admin";

        var ticket = await _db.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
        if (ticket is null) return null;

        var isOwner = ticket.CreatedByUserId == callerUserId;

        if (!isAgentOrAdmin && !isOwner)
        {
            throw new UnauthorizedAccessException("You can comment only on your own tickets.");
        }

        var comment = new TicketComment
        {
            TicketId = ticketId,
            UserId = callerUserId,
            Message = message.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _db.TicketComments.Add(comment);
        await _db.SaveChangesAsync();

        var userEmail = await _db.Users
            .Where(u => u.Id == callerUserId)
            .Select(u => u.Email)
            .FirstAsync();

        return new CommentResponse
        {
            Id = comment.Id,
            TicketId = comment.TicketId,
            UserId = comment.UserId,
            UserEmail = userEmail,
            Message = comment.Message,
            CreatedAt = comment.CreatedAt
        };
    }

    public async Task<List<CommentResponse>?> GetCommentsAsync(int ticketId, int callerUserId, string callerRole)
    {
        var isAgentOrAdmin = callerRole == "Agent" || callerRole == "Admin";

        var ticketQuery = _db.Tickets.Where(t => t.Id == ticketId);

        if (!isAgentOrAdmin)
        {
            ticketQuery = ticketQuery.Where(t => t.CreatedByUserId == callerUserId);
        }

        var canAccess = await ticketQuery.AnyAsync();
        if (!canAccess) return null;

        return await _db.TicketComments
            .Where(c => c.TicketId == ticketId)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new CommentResponse
            {
                Id = c.Id,
                TicketId = c.TicketId,
                UserId = c.UserId,
                UserEmail = c.User!.Email,
                Message = c.Message,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();
    }
}