using Training.Angular.Api.Application.Dtos;

namespace Training.Angular.Api.Application.Interfaces;

public interface IGetAccountByIdUseCase
{
    Task<AccountResponse?> ExecuteAsync(Guid id, CancellationToken ct);
}
