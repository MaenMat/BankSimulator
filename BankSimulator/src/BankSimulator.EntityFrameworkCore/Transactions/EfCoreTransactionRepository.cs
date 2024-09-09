using BankSimulator.Transactions;
using BankSimulator.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using BankSimulator.EntityFrameworkCore;

namespace BankSimulator.Transactions
{
    public class EfCoreTransactionRepository : EfCoreRepository<BankSimulatorDbContext, Transaction, Guid>, ITransactionRepository
    {
        public EfCoreTransactionRepository(IDbContextProvider<BankSimulatorDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<TransactionWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(transaction => new TransactionWithNavigationProperties
                {
                    Transaction = transaction,
                    Account = dbContext.Set<Account>().FirstOrDefault(c => c.Id == transaction.SourceAccountId),
                    Account1 = dbContext.Set<Account>().FirstOrDefault(c => c.Id == transaction.DestinationAccountId)
                }).FirstOrDefault();
        }

        public async Task<List<TransactionWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            TransactionType? transactionType = null,
            double? amountMin = null,
            double? amountMax = null,
            string description = null,
            DateTime? transactionDateMin = null,
            DateTime? transactionDateMax = null,
            TransactionStatus? transactionStatus = null,
            Guid? sourceAccountId = null,
            Guid? destinationAccountId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, transactionType, amountMin, amountMax, description, transactionDateMin, transactionDateMax, transactionStatus, sourceAccountId, destinationAccountId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? TransactionConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<TransactionWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from transaction in (await GetDbSetAsync())
                   join account in (await GetDbContextAsync()).Set<Account>() on transaction.SourceAccountId equals account.Id into accounts
                   from account in accounts.DefaultIfEmpty()
                   join account1 in (await GetDbContextAsync()).Set<Account>() on transaction.DestinationAccountId equals account1.Id into accounts1
                   from account1 in accounts1.DefaultIfEmpty()
                   select new TransactionWithNavigationProperties
                   {
                       Transaction = transaction,
                       Account = account,
                       Account1 = account1
                   };
        }

        protected virtual IQueryable<TransactionWithNavigationProperties> ApplyFilter(
            IQueryable<TransactionWithNavigationProperties> query,
            string filterText,
            TransactionType? transactionType = null,
            double? amountMin = null,
            double? amountMax = null,
            string description = null,
            DateTime? transactionDateMin = null,
            DateTime? transactionDateMax = null,
            TransactionStatus? transactionStatus = null,
            Guid? sourceAccountId = null,
            Guid? destinationAccountId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Transaction.Description.Contains(filterText))
                    .WhereIf(transactionType.HasValue, e => e.Transaction.TransactionType == transactionType)
                    .WhereIf(amountMin.HasValue, e => e.Transaction.Amount >= amountMin.Value)
                    .WhereIf(amountMax.HasValue, e => e.Transaction.Amount <= amountMax.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Transaction.Description.Contains(description))
                    .WhereIf(transactionDateMin.HasValue, e => e.Transaction.TransactionDate >= transactionDateMin.Value)
                    .WhereIf(transactionDateMax.HasValue, e => e.Transaction.TransactionDate <= transactionDateMax.Value)
                    .WhereIf(transactionStatus.HasValue, e => e.Transaction.TransactionStatus == transactionStatus)
                    .WhereIf(sourceAccountId != null && sourceAccountId != Guid.Empty, e => e.Account != null && e.Account.Id == sourceAccountId)
                    .WhereIf(destinationAccountId != null && destinationAccountId != Guid.Empty, e => e.Account1 != null && e.Account1.Id == destinationAccountId);
        }

        public async Task<List<Transaction>> GetListAsync(
            string filterText = null,
            TransactionType? transactionType = null,
            double? amountMin = null,
            double? amountMax = null,
            string description = null,
            DateTime? transactionDateMin = null,
            DateTime? transactionDateMax = null,
            TransactionStatus? transactionStatus = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, transactionType, amountMin, amountMax, description, transactionDateMin, transactionDateMax, transactionStatus);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? TransactionConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            TransactionType? transactionType = null,
            double? amountMin = null,
            double? amountMax = null,
            string description = null,
            DateTime? transactionDateMin = null,
            DateTime? transactionDateMax = null,
            TransactionStatus? transactionStatus = null,
            Guid? sourceAccountId = null,
            Guid? destinationAccountId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, transactionType, amountMin, amountMax, description, transactionDateMin, transactionDateMax, transactionStatus, sourceAccountId, destinationAccountId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Transaction> ApplyFilter(
            IQueryable<Transaction> query,
            string filterText,
            TransactionType? transactionType = null,
            double? amountMin = null,
            double? amountMax = null,
            string description = null,
            DateTime? transactionDateMin = null,
            DateTime? transactionDateMax = null,
            TransactionStatus? transactionStatus = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Description.Contains(filterText))
                    .WhereIf(transactionType.HasValue, e => e.TransactionType == transactionType)
                    .WhereIf(amountMin.HasValue, e => e.Amount >= amountMin.Value)
                    .WhereIf(amountMax.HasValue, e => e.Amount <= amountMax.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description.Contains(description))
                    .WhereIf(transactionDateMin.HasValue, e => e.TransactionDate >= transactionDateMin.Value)
                    .WhereIf(transactionDateMax.HasValue, e => e.TransactionDate <= transactionDateMax.Value)
                    .WhereIf(transactionStatus.HasValue, e => e.TransactionStatus == transactionStatus);
        }
    }
}