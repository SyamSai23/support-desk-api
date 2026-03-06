using Microsoft.EntityFrameworkCore;
using SupportDesk.Api.Data;
using SupportDesk.Api.Dtos.Tickets;
using SupportDesk.Api.Models;

namespace SupportDesk.Api.Services;

public class TicketService : ITicketService
{
    private readonly AppDbContext _db;

    public TicketService(AppDbContext db) => _db = db;

    public async Task<PagedResult<TicketResponse>> GetAllPagedAsync(
        int page,
        int pageSize,
        string? status,
        string? search,
        int callerUserId,
        string callerRole)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;
        pageSize = pageSize > 50 ? 50 : pageSize;

        var query = _db.Tickets.AsQueryable();

        var isAgentOrAdmin = callerRole == "Agent" || callerRole == "Admin";

        if (!isAgentOrAdmin)
        {
            query = query.Where(t => t.CreatedByUserId == callerUserId);
        }

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
                CreatedAt = t.CreatedAt,
                CreatedByUserId = t.CreatedByUserId,
                AssignedToUserId = t.AssignedToUserId,
                AssignedToUserEmail = t.AssignedToUser != null ? t.AssignedToUser.Email : null
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

    public async Task<PagedResult<TicketResponse>> GetAssignedToMeAsync(int page, int pageSize, int callerUserId, string callerRole)
    {
        if (callerRole != "Agent" && callerRole != "Admin")
        {
            throw new UnauthorizedAccessException("Only Agent or Admin can view assigned tickets.");
        }

        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;
        pageSize = pageSize > 50 ? 50 : pageSize;

        var query = _db.Tickets.Where(t => t.AssignedToUserId == callerUserId);

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
                CreatedAt = t.CreatedAt,
                CreatedByUserId = t.CreatedByUserId,
                AssignedToUserId = t.AssignedToUserId,
                AssignedToUserEmail = t.AssignedToUser != null ? t.AssignedToUser.Email : null
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

    public async Task<TicketResponse?> GetByIdAsync(int id, int callerUserId, string callerRole)
    {
        var isAgentOrAdmin = callerRole == "Agent" || callerRole == "Admin";

        var query = _db.Tickets.Where(t => t.Id == id);

        if (!isAgentOrAdmin)
        {
            query = query.Where(t => t.CreatedByUserId == callerUserId);
        }

        return await query
            .Select(t => new TicketResponse
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                CreatedByUserId = t.CreatedByUserId,
                AssignedToUserId = t.AssignedToUserId,
                AssignedToUserEmail = t.AssignedToUser != null ? t.AssignedToUser.Email : null
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
            CreatedAt = ticket.CreatedAt,
            CreatedByUserId = ticket.CreatedByUserId,
            AssignedToUserId = ticket.AssignedToUserId,
            AssignedToUserEmail = null
        };
    }

    public async Task<TicketResponse?> UpdateAsync(
        int id,
        UpdateTicketRequest req,
        int callerUserId,
        string callerRole)
    {
        var ticket = await _db.Tickets.FindAsync(id);
        if (ticket is null) return null;

        var isAgentOrAdmin = callerRole == "Agent" || callerRole == "Admin";
        var isOwner = ticket.CreatedByUserId == callerUserId;

        if (!isAgentOrAdmin && !isOwner)
        {
            throw new UnauthorizedAccessException("You can update only your own tickets.");
        }

        if (!isAgentOrAdmin && !string.Equals(ticket.Status, req.Status, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("Only Agent or Admin can change ticket status.");
        }

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
            CreatedAt = ticket.CreatedAt,
            CreatedByUserId = ticket.CreatedByUserId,
            AssignedToUserId = ticket.AssignedToUserId,
            AssignedToUserEmail = await _db.Users
                .Where(u => u.Id == ticket.AssignedToUserId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync()
        };
    }

    public async Task<bool> DeleteAsync(int id, int callerUserId, string callerRole)
    {
        var ticket = await _db.Tickets.FindAsync(id);
        if (ticket is null) return false;

        var isAgentOrAdmin = callerRole == "Agent" || callerRole == "Admin";
        var isOwner = ticket.CreatedByUserId == callerUserId;

        if (!isAgentOrAdmin && !isOwner)
        {
            throw new UnauthorizedAccessException("You can delete only your own tickets.");
        }

        _db.Tickets.Remove(ticket);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<TicketResponse?> AssignAsync(int id, int assignedToUserId, string callerRole)
    {
        if (callerRole != "Agent" && callerRole != "Admin")
        {
            throw new UnauthorizedAccessException("Only Agent or Admin can assign tickets.");
        }

        var ticket = await _db.Tickets.FindAsync(id);
        if (ticket is null) return null;

        var assignee = await _db.Users.FirstOrDefaultAsync(u =>
            u.Id == assignedToUserId && (u.Role == "Agent" || u.Role == "Admin"));

        if (assignee is null)
        {
            throw new InvalidOperationException("Assigned user must be an Agent or Admin.");
        }

        ticket.AssignedToUserId = assignedToUserId;
        await _db.SaveChangesAsync();

        return new TicketResponse
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Status = ticket.Status,
            CreatedAt = ticket.CreatedAt,
            CreatedByUserId = ticket.CreatedByUserId,
            AssignedToUserId = ticket.AssignedToUserId,
            AssignedToUserEmail = assignee.Email
        };
    }

    public async Task<List<(int Id, string Email)>> GetAssignableAgentsAsync(string callerRole)
    {
        if (callerRole != "Agent" && callerRole != "Admin")
        {
            throw new UnauthorizedAccessException("Only Agent or Admin can view assignable agents.");
        }

        return await _db.Users
            .Where(u => u.Role == "Agent" || u.Role == "Admin")
            .OrderBy(u => u.Email)
            .Select(u => new ValueTuple<int, string>(u.Id, u.Email))
            .ToListAsync();
    }
}