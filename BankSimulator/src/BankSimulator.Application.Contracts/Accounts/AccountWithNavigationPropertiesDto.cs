using BankSimulator.CustomerInfoFiles;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace BankSimulator.Accounts
{
    public class AccountWithNavigationPropertiesDto
    {
        public AccountDto Account { get; set; }

        public List<CustomerInfoFileDto> CustomerInfoFiles { get; set; }

    }
}