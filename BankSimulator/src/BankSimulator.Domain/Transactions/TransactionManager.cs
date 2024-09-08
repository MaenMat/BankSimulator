using BankSimulator.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace BankSimulator.Transactions
{
    public class TransactionManager : DomainService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionManager(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<Transaction> CreateAsync(
        Guid? sourceAccountId, Guid? destinationAccountId, TransactionType transactionType, double amount, string description, DateTime transactionDate)
        {
            Check.NotNull(transactionType, nameof(transactionType));
            Check.NotNull(transactionDate, nameof(transactionDate));

            var transaction = new Transaction(
             GuidGenerator.Create(),
             sourceAccountId, destinationAccountId, transactionType, amount, description, transactionDate
             );

            return await _transactionRepository.InsertAsync(transaction);
        }

        public async Task<Transaction> UpdateAsync(
            Guid id,
            Guid? sourceAccountId, Guid? destinationAccountId, TransactionType transactionType, double amount, string description, DateTime transactionDate, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNull(transactionType, nameof(transactionType));
            Check.NotNull(transactionDate, nameof(transactionDate));

            var transaction = await _transactionRepository.GetAsync(id);

            transaction.SourceAccountId = sourceAccountId;
            transaction.DestinationAccountId = destinationAccountId;
            transaction.TransactionType = transactionType;
            transaction.Amount = amount;
            transaction.Description = description;
            transaction.TransactionDate = transactionDate;

            transaction.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _transactionRepository.UpdateAsync(transaction);
        }

    }
}