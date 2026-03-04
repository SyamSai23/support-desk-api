using SupportDesk.Api.Dtos.Tickets;

namespace SupportDesk.Api.Services;

public interface ITicketService
{
    Task<PagedResult<TicketResponse>> GetAllPagedAsync(int page, int pageSize, string? status, string? search);
    Task<TicketResponse?> GetByIdAsync(int id);
    Task<TicketResponse> CreateAsync(CreateTicketRequest req, int createdByUserId);
    Task<TicketResponse?> UpdateAsync(int id, UpdateTicketRequest req);
    Task<bool> DeleteAsync(int id);
}