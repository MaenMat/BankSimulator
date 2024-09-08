using BankSimulator.Transactions;
using System;

namespace BankSimulator.Transactions
{
    public class TransactionExcelDto
    {
        public TransactionType TransactionType { get; set; }
        public double Amount { get; set; }
        public string? Description { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}