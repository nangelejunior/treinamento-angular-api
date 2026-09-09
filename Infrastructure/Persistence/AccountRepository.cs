using Microsoft.EntityFrameworkCore;
using Training.Angular.Api.Application.Interfaces;
using Training.Angular.Api.Domain.Entities;

namespace Training.Angular.Api.Infrastructure.Persistence;

public sealed class AccountRepository(ApplicationDbContext db) : IAccountRepository
{
    public async Task<IReadOnlyList<Account>> GetPagedAsync(int page, int pageSize, CancellationToken ct)
        => await db.Accounts
            .OrderBy(account => account.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task<Account?> FindByIdAsync(Guid id, CancellationToken ct)
        => await db.Accounts.FindAsync([id], ct);

    public async Task AddAsync(Account account, CancellationToken ct)
    {
        db.Accounts.Add(account);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Account account, CancellationToken ct)
    {
        db.Entry(account).State = EntityState.Modified;
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Account account, CancellationToken ct)
    {
        db.Accounts.Remove(account);
        await db.SaveChangesAsync(ct);
    }
}
