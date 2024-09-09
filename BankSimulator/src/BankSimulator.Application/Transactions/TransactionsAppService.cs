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
using Volo.Abp.Uow;

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
            var totalCount = await _transactionRepository.GetCountAsync(input.FilterText, input.TransactionType, input.AmountMin, input.AmountMax, input.Description, input.TransactionDateMin, input.TransactionDateMax, input.TransactionStatus, input.SourceAccountId, input.DestinationAccountId);
            var items = await _transactionRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.TransactionType, input.AmountMin, input.AmountMax, input.Description, input.TransactionDateMin, input.TransactionDateMax, input.TransactionStatus, input.SourceAccountId, input.DestinationAccountId, input.Sorting, input.MaxResultCount, input.SkipCount);

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

        //[Authorize(BankSimulatorPermissions.Transactions.Delete)]
        //public virtual async Task DeleteAsync(Guid id)
        //{
        //    await _transactionRepository.DeleteAsync(id);
        //}

        [Authorize(BankSimulatorPermissions.Transactions.Create)]
        [UnitOfWork]
        public virtual async Task<TransactionDto> CreateWithdrawAsync(WithdrawalCreateDto input)
        {
            if (input.Amount <= 0)
            {
                throw new UserFriendlyException(L["InvalidWithdrawAmount"]);
            }

            if (input.SourceAccountId == null) 
            {
                throw new UserFriendlyException(L["SourceAccountIdIsRequired"]);
            }

            var account = await _accountRepository.GetAsync(input.SourceAccountId.Value);
            if (account == null)
            {
                throw new UserFriendlyException(L["AccountNotFound"]);
            }
            
            if (!await ValidateWithdrawAmount(input.SourceAccountId,input.Amount))
            {
                throw new UserFriendlyException(L["InsuffecientAccountBalance"]);
            }

            account.Balance -= input.Amount;

            await _accountRepository.UpdateAsync(account);

            var transaction = await _transactionManager.CreateAsync(
            input.SourceAccountId, null, TransactionType.Withdrawal, input.Amount, input.Description, input.TransactionDate, TransactionStatus.Done
            );
            
            return ObjectMapper.Map<Transaction, TransactionDto>(transaction);
        }

        [Authorize(BankSimulatorPermissions.Transactions.Create)]
        [UnitOfWork]
        public virtual async Task<TransactionDto> CreateDepositAsync(DepositCreateDto input)
        {
            if (input.Amount <= 0)
            {
                throw new UserFriendlyException(L["InvalidDepositAmount"]);
            }

            if (input.DestinationAccountId == null)
            {
                throw new UserFriendlyException(L["DestinationAccountIdIsRequired"]);
            }

            var account = await _accountRepository.GetAsync(input.DestinationAccountId.Value);
            if (account == null)
            {
                throw new UserFriendlyException(L["AccountNotFound"]);
            }

            account.Balance += input.Amount;

            await _accountRepository.UpdateAsync(account);

            var transaction = await _transactionManager.CreateAsync(
            null, input.DestinationAccountId, TransactionType.Deposit, input.Amount, input.Description, input.TransactionDate, TransactionStatus.Done
            );

            return ObjectMapper.Map<Transaction, TransactionDto>(transaction);
        }

        [Authorize(BankSimulatorPermissions.Transactions.Create)]
        [UnitOfWork]
        public virtual async Task<TransactionDto> CreateTransferAsync(TransferCreateDto input)
        {
            if (input.Amount <= 0)
            {
                throw new UserFriendlyException(L["InvalidWithdrawAmount"]);
            }

            if (input.SourceAccountId == null)
            {
                throw new UserFriendlyException(L["SourceAccountIdIsRequired"]);
            }

            if (input.DestinationAccountId == null)
            {
                throw new UserFriendlyException(L["DestinationAccountIdIsRequired"]);
            }

            var sourceAccount = await _accountRepository.GetAsync(input.SourceAccountId.Value);
            if (sourceAccount == null)
            {
                throw new UserFriendlyException(L["SourceAccountNotFound"]);
            }

            var destinationAccount = await _accountRepository.GetAsync(input.DestinationAccountId.Value);
            if (destinationAccount == null)
            {
                throw new UserFriendlyException(L["DestinationAccountNotFound"]);
            }

            if (!await ValidateWithdrawAmount(input.SourceAccountId, input.Amount))
            {
                throw new UserFriendlyException(L["InsuffecientSourceAccountBalance"]);
            }

            sourceAccount.Balance -= input.Amount;
            destinationAccount.Balance += input.Amount;

            await _accountRepository.UpdateAsync(sourceAccount);
            await _accountRepository.UpdateAsync(destinationAccount);

            var transaction = await _transactionManager.CreateAsync(
           input.SourceAccountId, input.DestinationAccountId, TransactionType.Transfer, input.Amount, input.Description, input.TransactionDate, TransactionStatus.Done
           );

            return ObjectMapper.Map<Transaction, TransactionDto>(transaction);
        }

        [Authorize(BankSimulatorPermissions.Transactions.Edit)]
        public virtual async Task<TransactionDto> ReverseAsync(Guid id)
        {
            // Fetch the transaction by id
            var transaction = await _transactionRepository.GetAsync(id);
            if (transaction == null)
            {
                throw new UserFriendlyException(L["TransactionNotFound"]);
            }

            // Ensure the transaction hasn't been reversed already
            if (transaction.TransactionStatus == TransactionStatus.Reversed)
            {
                throw new UserFriendlyException(L["TransactionAlreadyReversed"]);
            }

            // Handle reversal based on transaction type
            switch (transaction.TransactionType)
            {
                case TransactionType.Withdrawal:
                    await ReverseWithdrawalAsync(transaction);
                    break;
                case TransactionType.Deposit:
                    await ReverseDepositAsync(transaction);
                    break;
                case TransactionType.Transfer:
                    await ReverseTransferAsync(transaction);
                    break;
                default:
                    throw new UserFriendlyException(L["InvalidTransactionType"]);
            }

            // Update transaction status to "Reversed"
            transaction.TransactionStatus = TransactionStatus.Reversed;
            await _transactionRepository.UpdateAsync(transaction);

            // Map to DTO and return the updated transaction
            return ObjectMapper.Map<Transaction, TransactionDto>(transaction);
        }

        // Helper method to reverse a withdrawal
        private async Task ReverseWithdrawalAsync(Transaction transaction)
        {
            var sourceAccount = await _accountRepository.GetAsync(transaction.SourceAccountId.Value);
            if (sourceAccount == null)
            {
                throw new UserFriendlyException(L["SourceAccountNotFound"]);
            }

            // Add the withdrawal amount back to the source account
            sourceAccount.Balance += transaction.Amount;

            // Save updated account balance
            await _accountRepository.UpdateAsync(sourceAccount);
        }

        // Helper method to reverse a deposit
        private async Task ReverseDepositAsync(Transaction transaction)
        {
            var destinationAccount = await _accountRepository.GetAsync(transaction.DestinationAccountId.Value);
            if (destinationAccount == null)
            {
                throw new UserFriendlyException(L["DestinationAccountNotFound"]);
            }

            // Subtract the deposit amount from the destination account
            destinationAccount.Balance -= transaction.Amount;

            // Save updated account balance
            await _accountRepository.UpdateAsync(destinationAccount);
        }

        // Helper method to reverse a transfer
        private async Task ReverseTransferAsync(Transaction transaction)
        {
            var sourceAccount = await _accountRepository.GetAsync(transaction.SourceAccountId.Value);
            var destinationAccount = await _accountRepository.GetAsync(transaction.DestinationAccountId.Value);

            if (sourceAccount == null)
            {
                throw new UserFriendlyException(L["SourceAccountNotFound"]);
            }

            if (destinationAccount == null)
            {
                throw new UserFriendlyException(L["DestinationAccountNotFound"]);
            }

            // Add the transfer amount back to the source account
            sourceAccount.Balance += transaction.Amount;

            // Subtract the transfer amount from the destination account
            destinationAccount.Balance -= transaction.Amount;

            // Save updated account balances
            await _accountRepository.UpdateAsync(sourceAccount);
            await _accountRepository.UpdateAsync(destinationAccount);
        }


        //[Authorize(BankSimulatorPermissions.Transactions.Edit)]
        //public virtual async Task<TransactionDto> UpdateAsync(Guid id, TransactionUpdateDto input)
        //{

        //    var transaction = await _transactionManager.UpdateAsync(
        //    id,
        //    input.SourceAccountId, input.DestinationAccountId, input.TransactionType, input.Amount, input.Description, input.TransactionDate, input.TransactionStatus, input.ConcurrencyStamp
        //    );

        //    return ObjectMapper.Map<Transaction, TransactionDto>(transaction);
        //}

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(TransactionExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var transactions = await _transactionRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.TransactionType, input.AmountMin, input.AmountMax, input.Description, input.TransactionDateMin, input.TransactionDateMax, input.TransactionStatus);
            var items = transactions.Select(item => new
            {
                TransactionType = item.Transaction.TransactionType,
                Amount = item.Transaction.Amount,
                Description = item.Transaction.Description,
                TransactionDate = item.Transaction.TransactionDate,
                TransactionStatus = item.Transaction.TransactionStatus,

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

        public async Task<bool> ValidateWithdrawAmount(Guid? AccountId, double Amount)
        { 
            var account = await _accountRepository.GetAsync(AccountId.GetValueOrDefault());   
            return account.Balance >= Amount ? true : false; 
        }
    }
}