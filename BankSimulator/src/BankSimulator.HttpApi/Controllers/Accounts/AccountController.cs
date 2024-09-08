using BankSimulator.Shared;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using BankSimulator.Accounts;
using Volo.Abp.Content;
using BankSimulator.Shared;
using System.Collections.Generic;

namespace BankSimulator.Controllers.Accounts
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Account")]
    [Route("api/app/accounts")]

    public class AccountController : AbpController, IAccountsAppService
    {
        private readonly IAccountsAppService _accountsAppService;

        public AccountController(IAccountsAppService accountsAppService)
        {
            _accountsAppService = accountsAppService;
        }

        [HttpGet]
        public Task<PagedResultDto<AccountWithNavigationPropertiesDto>> GetListAsync(GetAccountsInput input)
        {
            return _accountsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public Task<AccountWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _accountsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<AccountDto> GetAsync(Guid id)
        {
            return _accountsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("customer-info-file-lookup")]
        public Task<PagedResultDto<LookupDto<Guid>>> GetCustomerInfoFileLookupAsync(LookupRequestDto input)
        {
            return _accountsAppService.GetCustomerInfoFileLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<AccountDto> CreateAsync(AccountCreateDto input)
        {
            return _accountsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<AccountDto> UpdateAsync(Guid id, AccountUpdateDto input)
        {
            return _accountsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _accountsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(AccountExcelDownloadDto input)
        {
            return _accountsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _accountsAppService.GetDownloadTokenAsync();
        }
        [HttpGet]
        [Route("get-all-accounts")]
        public Task<object> GetAccountsByCIFNumberAsync(string cifNumber)
        {
            return _accountsAppService.GetAccountsByCIFNumberAsync(cifNumber);
        }
    }
}