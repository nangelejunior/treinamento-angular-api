using Training.Angular.Api.Domain.Enums;

namespace Training.Angular.Api.Application.Dtos;

public sealed record AccountRequest
{
    public AccountType AccountType { get; init; }

    public DateTime Date { get; init; }

    public string? Description { get; init; }

    public decimal Value { get; init; }
}
