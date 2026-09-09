using System.ComponentModel.DataAnnotations;

namespace Training.Angular.Api.Application.Dtos;

public sealed record AuthenticateRequest
{
    [Required]
    public string? Username { get; init; }

    [Required]
    public string? Password { get; init; }
}
