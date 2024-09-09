using BankSimulator.Shared;
using BankSimulator.CustomerInfoFiles;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using BankSimulator.Permissions;
using BankSimulator.Accounts;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using BankSimulator.Shared;

namespace BankSimulator.Accounts
{
    [RemoteService(IsEnabled = false)]
    [Authorize(BankSimulatorPermissions.Accounts.Default)]
    public class AccountsAppService : ApplicationService, IAccountsAppService
    {
        private readonly IDistributedCache<AccountExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly IAccountRepository _accountRepository;
        private readonly AccountManager _accountManager;
        private readonly IRepository<CustomerInfoFile, Guid> _customerInfoFileRepository;

        public AccountsAppService(IAccountRepository accountRepository, AccountManager accountManager, IDistributedCache<AccountExcelDownloadTokenCacheItem, string> excelDownloadTokenCache, IRepository<CustomerInfoFile, Guid> customerInfoFileRepository)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _accountRepository = accountRepository;
            _accountManager = accountManager; _customerInfoFileRepository = customerInfoFileRepository;
        }

        public virtual async Task<PagedResultDto<AccountWithNavigationPropertiesDto>> GetListAsync(GetAccountsInput input)
        {
            var totalCount = await _accountRepository.GetCountAsync(input.FilterText, input.AccountNumber, input.BalanceMin, input.BalanceMax, input.CustomerInfoFileId);
            var items = await _accountRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.AccountNumber, input.BalanceMin, input.BalanceMax, input.CustomerInfoFileId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<AccountWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<AccountWithNavigationProperties>, List<AccountWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<AccountWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<AccountWithNavigationProperties, AccountWithNavigationPropertiesDto>
                (await _accountRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<AccountDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Account, AccountDto>(await _accountRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetCustomerInfoFileLookupAsync(LookupRequestDto input)
        {
            var query = (await _customerInfoFileRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.CIFNumber != null &&
                         x.CIFNumber.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<CustomerInfoFile>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<CustomerInfoFile>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(BankSimulatorPermissions.Accounts.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _accountRepository.DeleteAsync(id);
        }

        [Authorize(BankSimulatorPermissions.Accounts.Create)]
        public virtual async Task<AccountDto> CreateAsync(AccountCreateDto input)
        {
            if(input.CustomerInfoFileIds.Count <= 0)
            {
                throw new UserFriendlyException(L["Can'tCreateBankAccountWithoutAtLeastOneCustomer"]);
            }
            var account = await _accountManager.CreateAsync(
            input.CustomerInfoFileIds, input.AccountNumber, input.Balance
            );

            return ObjectMapper.Map<Account, AccountDto>(account);
        }

        [Authorize(BankSimulatorPermissions.Accounts.Edit)]
        public virtual async Task<AccountDto> UpdateAsync(Guid id, AccountUpdateDto input)
        {
            if (input.CustomerInfoFileIds.Count <= 0)
            {
                throw new UserFriendlyException(L["Can'tEditBankAccountWithoutAtLeastOneCustomer"]);
            }
            var account = await _accountManager.UpdateAsync(
            id,
            input.CustomerInfoFileIds, input.AccountNumber, input.Balance, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Account, AccountDto>(account);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(AccountExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var accounts = await _accountRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.AccountNumber, input.BalanceMin, input.BalanceMax);
            var items = accounts.Select(item => new
            {
                AccountNumber = item.Account.AccountNumber,
                Balance = item.Account.Balance,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Accounts.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new AccountExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new DownloadTokenResultDto
            {
                Token = token
            };
        }

        public async Task<object> GetAccountsByCIFNumberAsync(string cifNumber)
        {
            //var customerQuery = await _customerInfoFileRepository.GetQueryableAsync();
            //var customer = customerQuery.Where(c => c.CIFNumber == cifNumber).FirstOrDefault();
            //if (customer == null) 
            //{
            //    throw new UserFriendlyException(L["NoCustomerWithThisCIFNumberWasFound"]); ;
            //} 
            //var query = await _accountRepository.GetQueryableAsync();
            //var accountIds = query.Where(account => account.CustomerInfoFiles
            //        .Any(cif => cif.CustomerInfoFileId == customer.Id))
            //    .Select(account => account.Id).ToList();
            //return accountIds;

            var accountQuery = await _accountRepository.GetQueryableAsync();
            var customerInfoFileQuery = await _customerInfoFileRepository.GetQueryableAsync();
            var accountIds = accountQuery
                .Where(account => account.CustomerInfoFiles
                    .Any(acf => customerInfoFileQuery
                        .Where(cif => cif.Id == acf.CustomerInfoFileId && cif.CIFNumber == cifNumber)
                        .Any()))
                .Select(account => new 
                {
                    AccountNo = account.AccountNumber,
                    Balance = account.Balance,
                }).ToList();

            if (!accountIds.Any())
            {
                throw new UserFriendlyException(L["NoAccountsWithThisCIFNumberWasFound"]);
            }
            return accountIds;
        }
    }
}