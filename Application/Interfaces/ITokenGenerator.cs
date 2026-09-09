using Training.Angular.Api.Application.Common;

namespace Training.Angular.Api.Application.Interfaces;

public interface ITokenGenerator
{
    Task<GeneratedToken> GenerateAsync(string username, string role, CancellationToken ct);
}
