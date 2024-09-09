using BankSimulator.Transactions;
using BankSimulator.Accounts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace BankSimulator.Transactions
{
    public class Transaction : FullAuditedAggregateRoot<Guid>
    {
        public virtual TransactionType TransactionType { get; set; }

        public virtual double Amount { get; set; }

        [CanBeNull]
        public virtual string? Description { get; set; }

        public virtual DateTime TransactionDate { get; set; }

        public virtual TransactionStatus TransactionStatus { get; set; }
        public Guid? SourceAccountId { get; set; }
        public Guid? DestinationAccountId { get; set; }

        public Transaction()
        {

        }

        public Transaction(Guid id, Guid? sourceAccountId, Guid? destinationAccountId, TransactionType transactionType, double amount, string description, DateTime transactionDate, TransactionStatus transactionStatus)
        {

            Id = id;
            TransactionType = transactionType;
            Amount = amount;
            Description = description;
            TransactionDate = transactionDate;
            TransactionStatus = transactionStatus;
            SourceAccountId = sourceAccountId;
            DestinationAccountId = destinationAccountId;
        }

    }
}