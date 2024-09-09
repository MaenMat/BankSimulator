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

namespace BankSimulator.Otps
{
    public class EfCoreOtpRepository : EfCoreRepository<BankSimulatorDbContext, Otp, Guid>, IOtpRepository
    {
        public EfCoreOtpRepository(IDbContextProvider<BankSimulatorDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<List<Otp>> GetListAsync(
            string filterText = null,
            string transactionNumber = null,
            string code = null,
            DateTime? expiryDateMin = null,
            DateTime? expiryDateMax = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, transactionNumber, code, expiryDateMin, expiryDateMax);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? OtpConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string transactionNumber = null,
            string code = null,
            DateTime? expiryDateMin = null,
            DateTime? expiryDateMax = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, transactionNumber, code, expiryDateMin, expiryDateMax);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Otp> ApplyFilter(
            IQueryable<Otp> query,
            string filterText,
            string transactionNumber = null,
            string code = null,
            DateTime? expiryDateMin = null,
            DateTime? expiryDateMax = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.TransactionNumber.Contains(filterText) || e.Code.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(transactionNumber), e => e.TransactionNumber.Contains(transactionNumber))
                    .WhereIf(!string.IsNullOrWhiteSpace(code), e => e.Code.Contains(code))
                    .WhereIf(expiryDateMin.HasValue, e => e.ExpiryDate >= expiryDateMin.Value)
                    .WhereIf(expiryDateMax.HasValue, e => e.ExpiryDate <= expiryDateMax.Value);
        }
    }
}