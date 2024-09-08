using Volo.Abp.Application.Dtos;
using System;

namespace BankSimulator.Accounts
{
    public class GetAccountsInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }

        public string? AccountNumber { get; set; }
        public double? BalanceMin { get; set; }
        public double? BalanceMax { get; set; }
        public Guid? CustomerInfoFileId { get; set; }

        public GetAccountsInput()
        {

        }
    }
}