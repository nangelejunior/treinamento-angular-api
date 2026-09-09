using Microsoft.EntityFrameworkCore;
using Training.Angular.Api.Application.Interfaces;
using Training.Angular.Api.Domain.Entities;

namespace Training.Angular.Api.Infrastructure.Persistence;

public sealed class UserRepository(ApplicationDbContext db) : IUserRepository
{
    public Task<User?> FindByUsernameAsync(string username, CancellationToken ct)
        => db.Users.FirstOrDefaultAsync(user => user.Username == username, ct);
}
