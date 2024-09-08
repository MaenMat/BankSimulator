using BankSimulator.Accounts;
using BankSimulator.Accounts;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace BankSimulator.Transactions
{
    public class TransactionWithNavigationPropertiesDto
    {
        public TransactionDto Transaction { get; set; }

        public AccountDto Account { get; set; }
        public AccountDto Account1 { get; set; }

    }
}