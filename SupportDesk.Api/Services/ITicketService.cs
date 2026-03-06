using SupportDesk.Api.Dtos.Tickets;

namespace SupportDesk.Api.Services;

public interface ITicketService
{
    Task<PagedResult<TicketResponse>> GetAllPagedAsync(
        int page,
        int pageSize,
        string? status,
        string? search,
        int callerUserId,
        string callerRole);

    Task<TicketResponse?> GetByIdAsync(int id, int callerUserId, string callerRole);

    Task<TicketResponse> CreateAsync(CreateTicketRequest req, int createdByUserId);

    Task<TicketResponse?> UpdateAsync(
        int id,
        UpdateTicketRequest req,
        int callerUserId,
        string callerRole);

    Task<bool> DeleteAsync(int id, int callerUserId, string callerRole);

    Task<TicketResponse?> AssignAsync(int id, int assignedToUserId, string callerRole);

    Task<List<(int Id, string Email)>> GetAssignableAgentsAsync(string callerRole);

    Task<PagedResult<TicketResponse>> GetAssignedToMeAsync(int page, int pageSize, int callerUserId, string callerRole);
}