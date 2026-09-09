using Training.Angular.Api.Application.Dtos;
using Training.Angular.Api.Application.Interfaces;
using Training.Angular.Api.Domain.Entities;

namespace Training.Angular.Api.Application.Services;

public sealed class CreateAccountUseCase(IAccountRepository accounts) : ICreateAccountUseCase
{
    public async Task<AccountResponse> ExecuteAsync(AccountRequest request, CancellationToken ct)
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            AccountType = request.AccountType,
            Date = request.Date,
            Description = request.Description,
            Value = request.Value
        };

        await accounts.AddAsync(account, ct);

        return new AccountResponse(account.Id, account.AccountType, account.Date, account.Description, account.Value);
    }
}
