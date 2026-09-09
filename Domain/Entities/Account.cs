using System;
using Training.Angular.Api.Domain.Enums;

namespace Training.Angular.Api.Domain.Entities;

public class Account
{
    public Guid Id { get; set; }

    public AccountType AccountType { get; set; }

    public DateTime Date { get; set; }

    public string? Description { get; set; }

    public decimal Value { get; set; }
}
