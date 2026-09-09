using Training.Angular.Api.Application.Common;

namespace Training.Angular.Api.Application.Interfaces;

public interface IAuthenticateUserUseCase
{
    Task<GeneratedToken?> ExecuteAsync(string? username, string? password, CancellationToken ct);
}
