using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BankSimulator.Accounts
{
    public class AccountCreateDto
    {
        public string? AccountNumber { get; set; }
        public double Balance { get; set; }
        public List<Guid> CustomerInfoFileIds { get; set; }
    }
}