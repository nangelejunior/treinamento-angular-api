namespace Training.Angular.Api.Application.Interfaces;

public interface IDeleteAccountUseCase
{
    Task ExecuteAsync(Guid id, CancellationToken ct);
}
