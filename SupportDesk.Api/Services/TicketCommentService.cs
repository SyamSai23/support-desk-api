using Microsoft.EntityFrameworkCore;
using SupportDesk.Api.Data;
using SupportDesk.Api.Dtos.Tickets.Comments;
using SupportDesk.Api.Models;

namespace SupportDesk.Api.Services;

public class TicketCommentService : ITicketCommentService
{
    private readonly AppDbContext _db;

    public TicketCommentService(AppDbContext db) => _db = db;

    public async Task<CommentResponse?> AddCommentAsync(int ticketId, int userId, string message)
    {
        var ticketExists = await _db.Tickets.AnyAsync(t => t.Id == ticketId);
        if (!ticketExists) return null;

        var comment = new TicketComment
        {
            TicketId = ticketId,
            UserId = userId,
            Message = message.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _db.TicketComments.Add(comment);
        await _db.SaveChangesAsync();

        var userEmail = await _db.Users
            .Where(u => u.Id == userId)
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

    public async Task<List<CommentResponse>> GetCommentsAsync(int ticketId)
    {
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