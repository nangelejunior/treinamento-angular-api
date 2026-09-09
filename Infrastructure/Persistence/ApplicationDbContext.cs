using Microsoft.EntityFrameworkCore;
using Training.Angular.Api.Domain.Entities;

namespace Training.Angular.Api.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Account> Accounts => Set<Account>();
}
