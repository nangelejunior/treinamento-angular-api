using Training.Angular.Api.Domain.Enums;

namespace Training.Angular.Api.Application.Dtos;

public sealed record AccountResponse(
    Guid Id,
    AccountType AccountType,
    DateTime Date,
    string? Description,
    decimal Value);
