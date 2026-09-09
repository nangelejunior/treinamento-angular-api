using Training.Angular.Api.Application.Dtos;
using Training.Angular.Api.Application.Interfaces;

namespace Training.Angular.Api.Application.Services;

public sealed class UpdateAccountUseCase(IAccountRepository accounts) : IUpdateAccountUseCase
{
    public async Task ExecuteAsync(Guid id, AccountRequest request, CancellationToken ct)
    {
        var account = await accounts.FindByIdAsync(id, ct);
        if (account is null)
            return;

        account.AccountType = request.AccountType;
        account.Date = request.Date;
        account.Description = request.Description;
        account.Value = request.Value;

        await accounts.UpdateAsync(account, ct);
    }
}
