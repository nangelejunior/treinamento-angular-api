using Training.Angular.Api.Application.Dtos;

namespace Training.Angular.Api.Application.Interfaces;

public interface ICreateAccountUseCase
{
    Task<AccountResponse> ExecuteAsync(AccountRequest request, CancellationToken ct);
}
