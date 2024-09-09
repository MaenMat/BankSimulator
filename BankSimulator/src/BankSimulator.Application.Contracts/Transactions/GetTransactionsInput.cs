using BankSimulator.Transactions;
using Volo.Abp.Application.Dtos;
using System;

namespace BankSimulator.Transactions
{
    public class GetTransactionsInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }

        public TransactionType? TransactionType { get; set; }
        public double? AmountMin { get; set; }
        public double? AmountMax { get; set; }
        public string? Description { get; set; }
        public DateTime? TransactionDateMin { get; set; }
        public DateTime? TransactionDateMax { get; set; }
        public TransactionStatus? TransactionStatus { get; set; }
        public Guid? SourceAccountId { get; set; }
        public Guid? DestinationAccountId { get; set; }

        public GetTransactionsInput()
        {

        }
    }
}