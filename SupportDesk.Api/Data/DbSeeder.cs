
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SupportDesk.Api.Models;

namespace SupportDesk.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAdminAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.Database.MigrateAsync();

        var adminEmail = "admin@supportdesk.com";

        var existingAdmin = await db.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
        if (existingAdmin is not null) return;

        var admin = new User
        {
            Email = adminEmail,
            Role = "Admin"
        };

        var hasher = new PasswordHasher<User>();
        admin.PasswordHash = hasher.HashPassword(admin, "Admin@12345");

        db.Users.Add(admin);
        await db.SaveChangesAsync();
    }
}