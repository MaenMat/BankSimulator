using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace BankSimulator.Otps
{
    public class OtpDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string? TransactionNumber { get; set; }
        public string? Code { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}