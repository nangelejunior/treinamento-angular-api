using Training.Angular.Api.Application.Dtos;

namespace Training.Angular.Api.Application.Interfaces;

public interface IUpdateAccountUseCase
{
    Task ExecuteAsync(Guid id, AccountRequest request, CancellationToken ct);
}
