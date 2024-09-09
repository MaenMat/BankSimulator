using BankSimulator.Transactions;
using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace BankSimulator.Transactions
{
    public class TransactionDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public TransactionType TransactionType { get; set; }
        public double Amount { get; set; }
        public string? Description { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
        public Guid? SourceAccountId { get; set; }
        public Guid? DestinationAccountId { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}