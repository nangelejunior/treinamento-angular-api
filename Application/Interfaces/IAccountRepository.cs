using Training.Angular.Api.Domain.Entities;

namespace Training.Angular.Api.Application.Interfaces;

public interface IAccountRepository
{
    Task<IReadOnlyList<Account>> GetPagedAsync(int page, int pageSize, CancellationToken ct);

    Task<Account?> FindByIdAsync(Guid id, CancellationToken ct);

    Task AddAsync(Account account, CancellationToken ct);

    Task UpdateAsync(Account account, CancellationToken ct);

    Task DeleteAsync(Account account, CancellationToken ct);
}
