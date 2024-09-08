using BankSimulator.CustomerInfoFiles;

using System;
using System.Collections.Generic;

namespace BankSimulator.Accounts
{
    public class AccountWithNavigationProperties
    {
        public Account Account { get; set; }

        

        public List<CustomerInfoFile> CustomerInfoFiles { get; set; }
        
    }
}