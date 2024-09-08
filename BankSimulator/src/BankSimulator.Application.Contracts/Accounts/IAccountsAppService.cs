using BankSimulator.Shared;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using BankSimulator.Shared;
using System.Collections.Generic;

namespace BankSimulator.Accounts
{
    public interface IAccountsAppService : IApplicationService
    {
        Task<PagedResultDto<AccountWithNavigationPropertiesDto>> GetListAsync(GetAccountsInput input);

        Task<AccountWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<AccountDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetCustomerInfoFileLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<AccountDto> CreateAsync(AccountCreateDto input);

        Task<AccountDto> UpdateAsync(Guid id, AccountUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(AccountExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
        Task<object> GetAccountsByCIFNumberAsync(string cifNumber);
    }
}