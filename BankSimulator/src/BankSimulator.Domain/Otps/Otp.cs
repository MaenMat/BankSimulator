using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace BankSimulator.Otps
{
    public class Otp : FullAuditedAggregateRoot<Guid>
    {
        [CanBeNull]
        public virtual string? TransactionNumber { get; set; }

        [CanBeNull]
        public virtual string? Code { get; set; }

        public virtual DateTime? ExpiryDate { get; set; }

        public Otp()
        {

        }

        public Otp(Guid id, string transactionNumber, string code, DateTime? expiryDate = null)
        {

            Id = id;
            TransactionNumber = transactionNumber;
            Code = code;
            ExpiryDate = expiryDate;
        }

    }
}