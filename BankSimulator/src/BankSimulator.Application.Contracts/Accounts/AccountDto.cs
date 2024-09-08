using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace BankSimulator.Accounts
{
    public class AccountDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string? AccountNumber { get; set; }
        public double Balance { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}