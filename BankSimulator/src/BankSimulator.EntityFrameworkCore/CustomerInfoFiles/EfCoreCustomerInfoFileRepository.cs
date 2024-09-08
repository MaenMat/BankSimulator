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

namespace BankSimulator.CustomerInfoFiles
{
    public class EfCoreCustomerInfoFileRepository : EfCoreRepository<BankSimulatorDbContext, CustomerInfoFile, Guid>, ICustomerInfoFileRepository
    {
        public EfCoreCustomerInfoFileRepository(IDbContextProvider<BankSimulatorDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<List<CustomerInfoFile>> GetListAsync(
            string filterText = null,
            string cIFNumber = null,
            string customerFirstName = null,
            string customerLastName = null,
            string phoneNumber = null,
            string nationalNumber = null,
            string customerAddress = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, cIFNumber, customerFirstName, customerLastName, phoneNumber, nationalNumber, customerAddress);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? CustomerInfoFileConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string cIFNumber = null,
            string customerFirstName = null,
            string customerLastName = null,
            string phoneNumber = null,
            string nationalNumber = null,
            string customerAddress = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, cIFNumber, customerFirstName, customerLastName, phoneNumber, nationalNumber, customerAddress);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<CustomerInfoFile> ApplyFilter(
            IQueryable<CustomerInfoFile> query,
            string filterText,
            string cIFNumber = null,
            string customerFirstName = null,
            string customerLastName = null,
            string phoneNumber = null,
            string nationalNumber = null,
            string customerAddress = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.CIFNumber.Contains(filterText) || e.CustomerFirstName.Contains(filterText) || e.CustomerLastName.Contains(filterText) || e.PhoneNumber.Contains(filterText) || e.NationalNumber.Contains(filterText) || e.CustomerAddress.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(cIFNumber), e => e.CIFNumber.Contains(cIFNumber))
                    .WhereIf(!string.IsNullOrWhiteSpace(customerFirstName), e => e.CustomerFirstName.Contains(customerFirstName))
                    .WhereIf(!string.IsNullOrWhiteSpace(customerLastName), e => e.CustomerLastName.Contains(customerLastName))
                    .WhereIf(!string.IsNullOrWhiteSpace(phoneNumber), e => e.PhoneNumber.Contains(phoneNumber))
                    .WhereIf(!string.IsNullOrWhiteSpace(nationalNumber), e => e.NationalNumber.Contains(nationalNumber))
                    .WhereIf(!string.IsNullOrWhiteSpace(customerAddress), e => e.CustomerAddress.Contains(customerAddress));
        }
    }
}