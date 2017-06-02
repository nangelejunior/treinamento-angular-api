using System;

namespace Treinamento.Angular.Api.Models
{
    public class Account
    {
        public Guid Id { get; set; }

        public EAccountType AccountType { get; set; }

        public DateTime Date { get; set; }        

        public string Description { get; set; }

        public decimal Value { get; set; }
    }

    public enum EAccountType
    {
        Payable = 0,
        Receivable
    }
}