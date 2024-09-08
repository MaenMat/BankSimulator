using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace BankSimulator.Accounts
{
    public class AccountUpdateDto : IHasConcurrencyStamp
    {
        public string? AccountNumber { get; set; }
        public double Balance { get; set; }
        public List<Guid> CustomerInfoFileIds { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}