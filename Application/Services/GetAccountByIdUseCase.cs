using Training.Angular.Api.Application.Dtos;
using Training.Angular.Api.Application.Interfaces;

namespace Training.Angular.Api.Application.Services;

public sealed class GetAccountByIdUseCase(IAccountRepository accounts) : IGetAccountByIdUseCase
{
    public async Task<AccountResponse?> ExecuteAsync(Guid id, CancellationToken ct)
    {
        var account = await accounts.FindByIdAsync(id, ct);

        return account is null
            ? null
            : new AccountResponse(account.Id, account.AccountType, account.Date, account.Description, account.Value);
    }
}
