using SupportDesk.Api.Dtos.Tickets.Comments;

namespace SupportDesk.Api.Services;

public interface ITicketCommentService
{
    Task<CommentResponse?> AddCommentAsync(int ticketId, int callerUserId, string callerRole, string message);
    Task<List<CommentResponse>?> GetCommentsAsync(int ticketId, int callerUserId, string callerRole);
}