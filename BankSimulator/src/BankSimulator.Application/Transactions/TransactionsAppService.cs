using BankSimulator.Shared;
using BankSimulator.Accounts;
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
using BankSimulator.Transactions;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using BankSimulator.Shared;

namespace BankSimulator.Transactions
{
    [RemoteService(IsEnabled = false)]
    [Authorize(BankSimulatorPermissions.Transactions.Default)]
    public class TransactionsAppService : ApplicationService, ITransactionsAppService
    {
        private readonly IDistributedCache<TransactionExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly ITransactionRepository _transactionRepository;
        private readonly TransactionManager _transactionManager;
        private readonly IRepository<Account, Guid> _accountRepository;

        public TransactionsAppService(ITransactionRepository transactionRepository, TransactionManager transactionManager, IDistributedCache<TransactionExcelDownloadTokenCacheItem, string> excelDownloadTokenCache, IRepository<Account, Guid> accountRepository)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _transactionRepository = transactionRepository;
            _transactionManager = transactionManager; _accountRepository = accountRepository;
        }

        public virtual async Task<PagedResultDto<TransactionWithNavigationPropertiesDto>> GetListAsync(GetTransactionsInput input)
        {
            var totalCount = await _transactionRepository.GetCountAsync(input.FilterText, input.TransactionType, input.AmountMin, input.AmountMax, input.Description, input.TransactionDateMin, input.TransactionDateMax, input.SourceAccountId, input.DestinationAccountId);
            var items = await _transactionRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.TransactionType, input.AmountMin, input.AmountMax, input.Description, input.TransactionDateMin, input.TransactionDateMax, input.SourceAccountId, input.DestinationAccountId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<TransactionWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<TransactionWithNavigationProperties>, List<TransactionWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<TransactionWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<TransactionWithNavigationProperties, TransactionWithNavigationPropertiesDto>
                (await _transactionRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<TransactionDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Transaction, TransactionDto>(await _transactionRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetAccountLookupAsync(LookupRequestDto input)
        {
            var query = (await _accountRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.AccountNumber != null &&
                         x.AccountNumber.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Account>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Account>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(BankSimulatorPermissions.Transactions.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _transactionRepository.DeleteAsync(id);
        }

        [Authorize(BankSimulatorPermissions.Transactions.Create)]
        public virtual async Task<TransactionDto> CreateAsync(TransactionCreateDto input)
        {

            var transaction = await _transactionManager.CreateAsync(
            input.SourceAccountId, input.DestinationAccountId, input.TransactionType, input.Amount, input.Description, input.TransactionDate
            );

            return ObjectMapper.Map<Transaction, TransactionDto>(transaction);
        }

        [Authorize(BankSimulatorPermissions.Transactions.Edit)]
        public virtual async Task<TransactionDto> UpdateAsync(Guid id, TransactionUpdateDto input)
        {

            var transaction = await _transactionManager.UpdateAsync(
            id,
            input.SourceAccountId, input.DestinationAccountId, input.TransactionType, input.Amount, input.Description, input.TransactionDate, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Transaction, TransactionDto>(transaction);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(TransactionExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var transactions = await _transactionRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.TransactionType, input.AmountMin, input.AmountMax, input.Description, input.TransactionDateMin, input.TransactionDateMax);
            var items = transactions.Select(item => new
            {
                TransactionType = item.Transaction.TransactionType,
                Amount = item.Transaction.Amount,
                Description = item.Transaction.Description,
                TransactionDate = item.Transaction.TransactionDate,

                SourceAccount = item.Account?.AccountNumber,
                DestinationAccount = item.Account1?.AccountNumber,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Transactions.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new TransactionExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new DownloadTokenResultDto
            {
                Token = token
            };
        }
    }
}