using BankSimulator.Transactions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BankSimulator.Transactions
{
    public class TransactionCreateDto
    {
        public TransactionType TransactionType { get; set; } = ((TransactionType[])Enum.GetValues(typeof(TransactionType)))[0];
        public double Amount { get; set; }
        public string? Description { get; set; }
        public DateTime TransactionDate { get; set; }
        public Guid? SourceAccountId { get; set; }
        public Guid? DestinationAccountId { get; set; }
    }
}