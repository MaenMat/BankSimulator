using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BankSimulator.CustomerInfoFiles
{
    public interface ICustomerInfoFileRepository : IRepository<CustomerInfoFile, Guid>
    {
        Task<List<CustomerInfoFile>> GetListAsync(
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
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string filterText = null,
            string cIFNumber = null,
            string customerFirstName = null,
            string customerLastName = null,
            string phoneNumber = null,
            string nationalNumber = null,
            string customerAddress = null,
            CancellationToken cancellationToken = default);
    }
}