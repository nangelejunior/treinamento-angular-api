using Training.Angular.Api.Application.Common;
using Training.Angular.Api.Application.Dtos;

namespace Training.Angular.Api.Application.Interfaces;

public interface IGetAccountsUseCase
{
    Task<PageResult<AccountResponse>> ExecuteAsync(int page, int pageSize, CancellationToken ct);
}
