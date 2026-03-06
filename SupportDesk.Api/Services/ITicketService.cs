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

    Task<TicketResponse?> UpdateAsync(int id, UpdateTicketRequest req, string callerRole);

    Task<bool> DeleteAsync(int id);
}