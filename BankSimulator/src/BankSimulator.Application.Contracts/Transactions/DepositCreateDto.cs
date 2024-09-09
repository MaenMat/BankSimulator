using System;
using System.Collections.Generic;
using System.Text;

namespace BankSimulator.Transactions
{
    public class DepositCreateDto
    {
        public double Amount { get; set; }
        public string? Description { get; set; }
        public DateTime TransactionDate { get; set; }
        public Guid? DestinationAccountId { get; set; }
    }
}
