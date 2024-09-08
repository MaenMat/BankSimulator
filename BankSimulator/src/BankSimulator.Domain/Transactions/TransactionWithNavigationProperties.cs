using BankSimulator.Accounts;
using BankSimulator.Accounts;

using System;
using System.Collections.Generic;

namespace BankSimulator.Transactions
{
    public class TransactionWithNavigationProperties
    {
        public Transaction Transaction { get; set; }

        public Account Account { get; set; }
        public Account Account1 { get; set; }
        

        
    }
}