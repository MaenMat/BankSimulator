using BankSimulator.Shared;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using BankSimulator.Shared;
using BankSimulator.Otps;

namespace BankSimulator.Transactions
{
    public interface ITransactionsAppService : IApplicationService
    {
        Task<PagedResultDto<TransactionWithNavigationPropertiesDto>> GetListAsync(GetTransactionsInput input);
        Task<TransactionWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);
        Task<TransactionDto> GetAsync(Guid id);
        Task<PagedResultDto<LookupDto<Guid>>> GetAccountLookupAsync(LookupRequestDto input);
        //Task DeleteAsync(Guid id);
        Task<IRemoteStreamContent> GetListAsExcelFileAsync(TransactionExcelDownloadDto input);
        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
        Task<TransactionDto> CreateWithdrawAsync(WithdrawalCreateDto input);
        Task<TransactionDto> CreateDepositAsync(DepositCreateDto input);
        Task<TransactionDto> CreateTransferAsync(TransferCreateDto input);
        Task<TransactionDto> ReverseAsync(Guid id);
        Task<string> CreateWithdrawRequestAsync(WithdrawalCreateDto input);
        Task<string> ConfirmWithdrawRequestAsync(ConfirmOptDto input);

    }
}