using Training.Angular.Api.Application.Common;
using Training.Angular.Api.Application.Interfaces;

namespace Training.Angular.Api.Application.Services;

public sealed class AuthenticateUserUseCase(IUserRepository users, ITokenGenerator tokenGenerator) : IAuthenticateUserUseCase
{
    public async Task<GeneratedToken?> ExecuteAsync(string? username, string? password, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return null;

        var user = await users.FindByUsernameAsync(username, ct);
        if (user is null || user.Password != password)
            return null;

        return await tokenGenerator.GenerateAsync(user.Username, user.Role, ct);
    }
}
