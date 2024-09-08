using BankSimulator.Shared;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using BankSimulator.Shared;

namespace BankSimulator.Transactions
{
    public interface ITransactionsAppService : IApplicationService
    {
        Task<PagedResultDto<TransactionWithNavigationPropertiesDto>> GetListAsync(GetTransactionsInput input);

        Task<TransactionWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<TransactionDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetAccountLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<TransactionDto> CreateAsync(TransactionCreateDto input);

        Task<TransactionDto> UpdateAsync(Guid id, TransactionUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(TransactionExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}