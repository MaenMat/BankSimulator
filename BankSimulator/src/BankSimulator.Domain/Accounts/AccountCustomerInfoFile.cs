using System;
using Volo.Abp.Domain.Entities;

namespace BankSimulator.Accounts
{
    public class AccountCustomerInfoFile : Entity
    {

        public Guid AccountId { get; protected set; }

        public Guid CustomerInfoFileId { get; protected set; }

        private AccountCustomerInfoFile()
        {

        }

        public AccountCustomerInfoFile(Guid accountId, Guid customerInfoFileId)
        {
            AccountId = accountId;
            CustomerInfoFileId = customerInfoFileId;
        }

        public override object[] GetKeys()
        {
            return new object[]
                {
                    AccountId,
                    CustomerInfoFileId
                };
        }
    }
}