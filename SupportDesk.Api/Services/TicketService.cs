using Microsoft.EntityFrameworkCore;
using SupportDesk.Api.Data;
using SupportDesk.Api.Dtos.Tickets;
using SupportDesk.Api.Models;

namespace SupportDesk.Api.Services;

public class TicketService : ITicketService
{
    private readonly AppDbContext _db;

    public TicketService(AppDbContext db) => _db = db;

    public async Task<PagedResult<TicketResponse>> GetAllPagedAsync(int page, int pageSize, string? status, string? search)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;
        pageSize = pageSize > 50 ? 50 : pageSize; // safety cap

        var query = _db.Tickets.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(t => t.Status == status);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(t => t.Title.Contains(s) || t.Description.Contains(s));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TicketResponse
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResult<TicketResponse>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }

    public async Task<TicketResponse?> GetByIdAsync(int id)
    {
        return await _db.Tickets
            .Where(t => t.Id == id)
            .Select(t => new TicketResponse
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                CreatedAt = t.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<TicketResponse> CreateAsync(CreateTicketRequest req, int createdByUserId)
    {
        var ticket = new Ticket
        {
            Title = req.Title.Trim(),
            Description = req.Description.Trim(),
            Status = "Open",
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = createdByUserId
        };

        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync();

        return new TicketResponse
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Status = ticket.Status,
            CreatedAt = ticket.CreatedAt
        };
    }

    public async Task<TicketResponse?> UpdateAsync(int id, UpdateTicketRequest req)
    {
        var ticket = await _db.Tickets.FindAsync(id);
        if (ticket is null) return null;

        ticket.Title = req.Title.Trim();
        ticket.Description = req.Description.Trim();
        ticket.Status = req.Status;

        await _db.SaveChangesAsync();

        return new TicketResponse
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Status = ticket.Status,
            CreatedAt = ticket.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var ticket = await _db.Tickets.FindAsync(id);
        if (ticket is null) return false;

        _db.Tickets.Remove(ticket);
        await _db.SaveChangesAsync();
        return true;
    }
}