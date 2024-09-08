using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace BankSimulator.Accounts
{
    public class Account : FullAuditedAggregateRoot<Guid>
    {
        [CanBeNull]
        public virtual string? AccountNumber { get; set; }

        public virtual double Balance { get; set; }

        public ICollection<AccountCustomerInfoFile> CustomerInfoFiles { get; private set; }

        public Account()
        {

        }

        public Account(Guid id, string accountNumber, double balance)
        {

            Id = id;
            AccountNumber = accountNumber;
            Balance = balance;
            CustomerInfoFiles = new Collection<AccountCustomerInfoFile>();
        }
        public void AddCustomerInfoFile(Guid customerInfoFileId)
        {
            Check.NotNull(customerInfoFileId, nameof(customerInfoFileId));

            if (IsInCustomerInfoFiles(customerInfoFileId))
            {
                return;
            }

            CustomerInfoFiles.Add(new AccountCustomerInfoFile(Id, customerInfoFileId));
        }

        public void RemoveCustomerInfoFile(Guid customerInfoFileId)
        {
            Check.NotNull(customerInfoFileId, nameof(customerInfoFileId));

            if (!IsInCustomerInfoFiles(customerInfoFileId))
            {
                return;
            }

            CustomerInfoFiles.RemoveAll(x => x.CustomerInfoFileId == customerInfoFileId);
        }

        public void RemoveAllCustomerInfoFilesExceptGivenIds(List<Guid> customerInfoFileIds)
        {
            Check.NotNullOrEmpty(customerInfoFileIds, nameof(customerInfoFileIds));

            CustomerInfoFiles.RemoveAll(x => !customerInfoFileIds.Contains(x.CustomerInfoFileId));
        }

        public void RemoveAllCustomerInfoFiles()
        {
            CustomerInfoFiles.RemoveAll(x => x.AccountId == Id);
        }

        private bool IsInCustomerInfoFiles(Guid customerInfoFileId)
        {
            return CustomerInfoFiles.Any(x => x.CustomerInfoFileId == customerInfoFileId);
        }
    }
}