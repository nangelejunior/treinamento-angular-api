using Training.Angular.Api.Domain.Entities;

namespace Training.Angular.Api.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> FindByUsernameAsync(string username, CancellationToken ct);
}
