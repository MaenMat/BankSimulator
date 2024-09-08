using BankSimulator.CustomerInfoFiles;
using BankSimulator.CustomerInfoFiles;
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

namespace BankSimulator.Accounts
{
    public class EfCoreAccountRepository : EfCoreRepository<BankSimulatorDbContext, Account, Guid>, IAccountRepository
    {
        public EfCoreAccountRepository(IDbContextProvider<BankSimulatorDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<AccountWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id).Include(x => x.CustomerInfoFiles)
                .Select(account => new AccountWithNavigationProperties
                {
                    Account = account,
                    CustomerInfoFiles = (from accountCustomerInfoFiles in account.CustomerInfoFiles
                                         join _customerInfoFile in dbContext.Set<CustomerInfoFile>() on accountCustomerInfoFiles.CustomerInfoFileId equals _customerInfoFile.Id
                                         select _customerInfoFile).ToList()
                }).FirstOrDefault();
        }

        public async Task<List<AccountWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string accountNumber = null,
            double? balanceMin = null,
            double? balanceMax = null,
            Guid? customerInfoFileId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, accountNumber, balanceMin, balanceMax, customerInfoFileId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? AccountConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<AccountWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from account in (await GetDbSetAsync())

                   select new AccountWithNavigationProperties
                   {
                       Account = account,
                       CustomerInfoFiles = new List<CustomerInfoFile>()
                   };
        }

        protected virtual IQueryable<AccountWithNavigationProperties> ApplyFilter(
            IQueryable<AccountWithNavigationProperties> query,
            string filterText,
            string accountNumber = null,
            double? balanceMin = null,
            double? balanceMax = null,
            Guid? customerInfoFileId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Account.AccountNumber.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(accountNumber), e => e.Account.AccountNumber.Contains(accountNumber))
                    .WhereIf(balanceMin.HasValue, e => e.Account.Balance >= balanceMin.Value)
                    .WhereIf(balanceMax.HasValue, e => e.Account.Balance <= balanceMax.Value)
                    .WhereIf(customerInfoFileId != null && customerInfoFileId != Guid.Empty, e => e.Account.CustomerInfoFiles.Any(x => x.CustomerInfoFileId == customerInfoFileId));
        }

        public async Task<List<Account>> GetListAsync(
            string filterText = null,
            string accountNumber = null,
            double? balanceMin = null,
            double? balanceMax = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, accountNumber, balanceMin, balanceMax);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? AccountConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string accountNumber = null,
            double? balanceMin = null,
            double? balanceMax = null,
            Guid? customerInfoFileId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, accountNumber, balanceMin, balanceMax, customerInfoFileId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Account> ApplyFilter(
            IQueryable<Account> query,
            string filterText,
            string accountNumber = null,
            double? balanceMin = null,
            double? balanceMax = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.AccountNumber.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(accountNumber), e => e.AccountNumber.Contains(accountNumber))
                    .WhereIf(balanceMin.HasValue, e => e.Balance >= balanceMin.Value)
                    .WhereIf(balanceMax.HasValue, e => e.Balance <= balanceMax.Value);
        }
    }
}