using Training.Angular.Api.Application.Common;
using Training.Angular.Api.Application.Dtos;
using Training.Angular.Api.Application.Interfaces;

namespace Training.Angular.Api.Application.Services;

public sealed class GetAccountsUseCase(IAccountRepository accounts) : IGetAccountsUseCase
{
    public async Task<PageResult<AccountResponse>> ExecuteAsync(int page, int pageSize, CancellationToken ct)
    {
        (page, pageSize) = PageResult<AccountResponse>.Clamp(page, pageSize);

        var items = await accounts.GetPagedAsync(page, pageSize, ct);

        var responses = items
            .Select(account => new AccountResponse(account.Id, account.AccountType, account.Date, account.Description, account.Value))
            .ToList();

        return new PageResult<AccountResponse>(page, pageSize, responses);
    }
}
