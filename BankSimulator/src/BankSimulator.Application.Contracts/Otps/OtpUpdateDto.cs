using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace BankSimulator.Otps
{
    public class OtpUpdateDto : IHasConcurrencyStamp
    {
        public string? Code { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}