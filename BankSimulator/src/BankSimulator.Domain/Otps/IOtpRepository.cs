using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BankSimulator.Otps
{
    public interface IOtpRepository : IRepository<Otp, Guid>
    {
        Task<List<Otp>> GetListAsync(
            string filterText = null,
            string transactionNumber = null,
            string code = null,
            DateTime? expiryDateMin = null,
            DateTime? expiryDateMax = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string filterText = null,
            string transactionNumber = null,
            string code = null,
            DateTime? expiryDateMin = null,
            DateTime? expiryDateMax = null,
            CancellationToken cancellationToken = default);
    }
}