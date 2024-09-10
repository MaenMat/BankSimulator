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
        Guid? sourceAccountId, Guid? destinationAccountId, TransactionType transactionType, double amount, string description, DateTime transactionDate, TransactionStatus transactionStatus)
        {
            Check.NotNull(transactionType, nameof(transactionType));
            Check.NotNull(transactionDate, nameof(transactionDate));
            Check.NotNull(transactionStatus, nameof(transactionStatus));
            var TransNumber = await GenerateUniqueTransactionNumberAsync();
            var transaction = new Transaction(
             GuidGenerator.Create(),
             TransNumber,
             sourceAccountId, destinationAccountId, transactionType, amount, description, transactionDate, transactionStatus
             );

            return await _transactionRepository.InsertAsync(transaction);
        }

        public async Task<Transaction> UpdateAsync(
            Guid id,
            string transactionNumber,
            Guid? sourceAccountId, Guid? destinationAccountId, TransactionType transactionType, double amount, string description, DateTime transactionDate, TransactionStatus transactionStatus, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNull(transactionType, nameof(transactionType));
            Check.NotNull(transactionDate, nameof(transactionDate));
            Check.NotNull(transactionStatus, nameof(transactionStatus));

            var transaction = await _transactionRepository.GetAsync(id);

            transaction.TransactionNumber = transactionNumber;
            transaction.SourceAccountId = sourceAccountId;
            transaction.DestinationAccountId = destinationAccountId;
            transaction.TransactionType = transactionType;
            transaction.Amount = amount;
            transaction.Description = description;
            transaction.TransactionDate = transactionDate;
            transaction.TransactionStatus = transactionStatus;

            transaction.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _transactionRepository.UpdateAsync(transaction);
        }

        public async Task<string> GenerateUniqueTransactionNumberAsync()
        {
            Random random = new Random();
            string transactionNumber;

            bool isUnique;
            do
            {
                // Generates an 8-digit number
                int uniqueNumber = random.Next(10000000, 99999999);
                transactionNumber = uniqueNumber.ToString();

                // Check if this number already exists in the database
                isUnique = await _transactionRepository.FirstOrDefaultAsync(x => x.TransactionNumber == transactionNumber) == null;

            } while (!isUnique); // Keep generating until a unique number is found

            return transactionNumber;
        }

    }
}