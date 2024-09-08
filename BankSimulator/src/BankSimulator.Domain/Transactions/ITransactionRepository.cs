using BankSimulator.Transactions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BankSimulator.Transactions
{
    public interface ITransactionRepository : IRepository<Transaction, Guid>
    {
        Task<TransactionWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<TransactionWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            TransactionType? transactionType = null,
            double? amountMin = null,
            double? amountMax = null,
            string description = null,
            DateTime? transactionDateMin = null,
            DateTime? transactionDateMax = null,
            Guid? sourceAccountId = null,
            Guid? destinationAccountId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<Transaction>> GetListAsync(
                    string filterText = null,
                    TransactionType? transactionType = null,
                    double? amountMin = null,
                    double? amountMax = null,
                    string description = null,
                    DateTime? transactionDateMin = null,
                    DateTime? transactionDateMax = null,
                    string sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string filterText = null,
            TransactionType? transactionType = null,
            double? amountMin = null,
            double? amountMax = null,
            string description = null,
            DateTime? transactionDateMin = null,
            DateTime? transactionDateMax = null,
            Guid? sourceAccountId = null,
            Guid? destinationAccountId = null,
            CancellationToken cancellationToken = default);
    }
}