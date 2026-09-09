using Microsoft.EntityFrameworkCore;
using Training.Angular.Api.Domain.Entities;

namespace Training.Angular.Api.Infrastructure.Persistence;

public sealed class DatabaseSeeder(ApplicationDbContext db)
{
    public async Task SeedAsync(CancellationToken ct)
    {
        if (await db.Users.AnyAsync(ct))
            return;

        db.Users.AddRange(
            new User { Id = Guid.NewGuid(), Username = "neuclair.junior", Password = "admin123", Role = "Admin" },
            new User { Id = Guid.NewGuid(), Username = "felipe.milhossi", Password = "admin123", Role = "Admin" });

        await db.SaveChangesAsync(ct);
    }
}
