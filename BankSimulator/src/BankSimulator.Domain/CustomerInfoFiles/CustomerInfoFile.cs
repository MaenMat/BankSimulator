using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace BankSimulator.CustomerInfoFiles
{
    public class CustomerInfoFile : FullAuditedAggregateRoot<Guid>
    {
        [CanBeNull]
        public virtual string? CIFNumber { get; set; }

        [NotNull]
        public virtual string CustomerFirstName { get; set; }

        [NotNull]
        public virtual string CustomerLastName { get; set; }

        [NotNull]
        public virtual string PhoneNumber { get; set; }

        [NotNull]
        public virtual string NationalNumber { get; set; }

        [CanBeNull]
        public virtual string? CustomerAddress { get; set; }

        public CustomerInfoFile()
        {

        }

        public CustomerInfoFile(Guid id, string cIFNumber, string customerFirstName, string customerLastName, string phoneNumber, string nationalNumber, string customerAddress)
        {

            Id = id;
            Check.NotNull(customerFirstName, nameof(customerFirstName));
            Check.NotNull(customerLastName, nameof(customerLastName));
            Check.NotNull(phoneNumber, nameof(phoneNumber));
            Check.NotNull(nationalNumber, nameof(nationalNumber));
            CIFNumber = cIFNumber;
            CustomerFirstName = customerFirstName;
            CustomerLastName = customerLastName;
            PhoneNumber = phoneNumber;
            NationalNumber = nationalNumber;
            CustomerAddress = customerAddress;
        }

    }
}