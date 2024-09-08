using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BankSimulator.Accounts
{
    public interface IAccountRepository : IRepository<Account, Guid>
    {
        Task<AccountWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<AccountWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string accountNumber = null,
            double? balanceMin = null,
            double? balanceMax = null,
            Guid? customerInfoFileId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<Account>> GetListAsync(
                    string filterText = null,
                    string accountNumber = null,
                    double? balanceMin = null,
                    double? balanceMax = null,
                    string sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string filterText = null,
            string accountNumber = null,
            double? balanceMin = null,
            double? balanceMax = null,
            Guid? customerInfoFileId = null,
            CancellationToken cancellationToken = default);
    }
}