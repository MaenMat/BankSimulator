using BankSimulator.CustomerInfoFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace BankSimulator.Accounts
{
    public class AccountManager : DomainService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IRepository<CustomerInfoFile, Guid> _customerInfoFileRepository;

        public AccountManager(IAccountRepository accountRepository,
        IRepository<CustomerInfoFile, Guid> customerInfoFileRepository)
        {
            _accountRepository = accountRepository;
            _customerInfoFileRepository = customerInfoFileRepository;
        }

        public async Task<Account> CreateAsync(
        List<Guid> customerInfoFileIds,
        string accountNumber, double balance)
        {

            var account = new Account(
             GuidGenerator.Create(),
             accountNumber, balance
             );

            await SetCustomerInfoFilesAsync(account, customerInfoFileIds);

            return await _accountRepository.InsertAsync(account);
        }

        public async Task<Account> UpdateAsync(
            Guid id,
            List<Guid> customerInfoFileIds,
        string accountNumber, double balance, [CanBeNull] string concurrencyStamp = null
        )
        {

            var queryable = await _accountRepository.WithDetailsAsync(x => x.CustomerInfoFiles);
            var query = queryable.Where(x => x.Id == id);

            var account = await AsyncExecuter.FirstOrDefaultAsync(query);

            account.AccountNumber = accountNumber;
            account.Balance = balance;

            await SetCustomerInfoFilesAsync(account, customerInfoFileIds);

            account.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _accountRepository.UpdateAsync(account);
        }

        private async Task SetCustomerInfoFilesAsync(Account account, List<Guid> customerInfoFileIds)
        {
            if (customerInfoFileIds == null || !customerInfoFileIds.Any())
            {
                account.RemoveAllCustomerInfoFiles();
                return;
            }

            var query = (await _customerInfoFileRepository.GetQueryableAsync())
                .Where(x => customerInfoFileIds.Contains(x.Id))
                .Select(x => x.Id);

            var customerInfoFileIdsInDb = await AsyncExecuter.ToListAsync(query);
            if (!customerInfoFileIdsInDb.Any())
            {
                return;
            }

            account.RemoveAllCustomerInfoFilesExceptGivenIds(customerInfoFileIdsInDb);

            foreach (var customerInfoFileId in customerInfoFileIdsInDb)
            {
                account.AddCustomerInfoFile(customerInfoFileId);
            }
        }

    }
}