using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace BankSimulator.CustomerInfoFiles
{
    public class CustomerInfoFileManager : DomainService
    {
        private readonly ICustomerInfoFileRepository _customerInfoFileRepository;

        public CustomerInfoFileManager(ICustomerInfoFileRepository customerInfoFileRepository)
        {
            _customerInfoFileRepository = customerInfoFileRepository;
        }

        public async Task<CustomerInfoFile> CreateAsync(
        string cIFNumber, string customerFirstName, string customerLastName, string phoneNumber, string nationalNumber, string customerAddress)
        {
            Check.NotNullOrWhiteSpace(customerFirstName, nameof(customerFirstName));
            Check.NotNullOrWhiteSpace(customerLastName, nameof(customerLastName));
            Check.NotNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));
            Check.NotNullOrWhiteSpace(nationalNumber, nameof(nationalNumber));

            var customerInfoFile = new CustomerInfoFile(
             GuidGenerator.Create(),
             cIFNumber, customerFirstName, customerLastName, phoneNumber, nationalNumber, customerAddress
             );

            return await _customerInfoFileRepository.InsertAsync(customerInfoFile);
        }

        public async Task<CustomerInfoFile> UpdateAsync(
            Guid id,
            string cIFNumber, string customerFirstName, string customerLastName, string phoneNumber, string nationalNumber, string customerAddress, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(customerFirstName, nameof(customerFirstName));
            Check.NotNullOrWhiteSpace(customerLastName, nameof(customerLastName));
            Check.NotNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));
            Check.NotNullOrWhiteSpace(nationalNumber, nameof(nationalNumber));

            var customerInfoFile = await _customerInfoFileRepository.GetAsync(id);

            customerInfoFile.CIFNumber = cIFNumber;
            customerInfoFile.CustomerFirstName = customerFirstName;
            customerInfoFile.CustomerLastName = customerLastName;
            customerInfoFile.PhoneNumber = phoneNumber;
            customerInfoFile.NationalNumber = nationalNumber;
            customerInfoFile.CustomerAddress = customerAddress;

            customerInfoFile.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _customerInfoFileRepository.UpdateAsync(customerInfoFile);
        }

        //public static string GenerateCIFNumber(Guid inputGuid)
        //{
        //    string guidString = inputGuid.ToString().Replace("-", "");
        //    return guidString.Substring(0, 11);
        //}

    }
}