using Training.Angular.Api.Application.Interfaces;

namespace Training.Angular.Api.Application.Services;

public sealed class DeleteAccountUseCase(IAccountRepository accounts) : IDeleteAccountUseCase
{
    public async Task ExecuteAsync(Guid id, CancellationToken ct)
    {
        var account = await accounts.FindByIdAsync(id, ct);
        if (account is null)
            return;

        await accounts.DeleteAsync(account, ct);
    }
}
