using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using BankSimulator.Permissions;
using BankSimulator.CustomerInfoFiles;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using BankSimulator.Shared;

namespace BankSimulator.CustomerInfoFiles
{
    [RemoteService(IsEnabled = false)]
    [Authorize(BankSimulatorPermissions.CustomerInfoFiles.Default)]
    public class CustomerInfoFilesAppService : ApplicationService, ICustomerInfoFilesAppService
    {
        private readonly IDistributedCache<CustomerInfoFileExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly ICustomerInfoFileRepository _customerInfoFileRepository;
        private readonly CustomerInfoFileManager _customerInfoFileManager;

        public CustomerInfoFilesAppService(ICustomerInfoFileRepository customerInfoFileRepository, CustomerInfoFileManager customerInfoFileManager, IDistributedCache<CustomerInfoFileExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _customerInfoFileRepository = customerInfoFileRepository;
            _customerInfoFileManager = customerInfoFileManager;
        }

        public virtual async Task<PagedResultDto<CustomerInfoFileDto>> GetListAsync(GetCustomerInfoFilesInput input)
        {
            var totalCount = await _customerInfoFileRepository.GetCountAsync(input.FilterText, input.CIFNumber, input.CustomerFirstName, input.CustomerLastName, input.PhoneNumber, input.NationalNumber, input.CustomerAddress);
            var items = await _customerInfoFileRepository.GetListAsync(input.FilterText, input.CIFNumber, input.CustomerFirstName, input.CustomerLastName, input.PhoneNumber, input.NationalNumber, input.CustomerAddress, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<CustomerInfoFileDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<CustomerInfoFile>, List<CustomerInfoFileDto>>(items)
            };
        }

        public virtual async Task<CustomerInfoFileDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<CustomerInfoFile, CustomerInfoFileDto>(await _customerInfoFileRepository.GetAsync(id));
        }

        [Authorize(BankSimulatorPermissions.CustomerInfoFiles.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _customerInfoFileRepository.DeleteAsync(id);
        }

        [Authorize(BankSimulatorPermissions.CustomerInfoFiles.Create)]
        public virtual async Task<CustomerInfoFileDto> CreateAsync(CustomerInfoFileCreateDto input)
        {
            if (await _customerInfoFileRepository.AnyAsync(c => c.CIFNumber == input.CIFNumber))
            {
                throw new UserFriendlyException(L["CIFNumberMustBeUnique"]);
            }
            var customerInfoFile = await _customerInfoFileManager.CreateAsync(
            input.CIFNumber, input.CustomerFirstName, input.CustomerLastName, input.PhoneNumber, input.NationalNumber, input.CustomerAddress
            );

            return ObjectMapper.Map<CustomerInfoFile, CustomerInfoFileDto>(customerInfoFile);
        }

        [Authorize(BankSimulatorPermissions.CustomerInfoFiles.Edit)]
        public virtual async Task<CustomerInfoFileDto> UpdateAsync(Guid id, CustomerInfoFileUpdateDto input)
        {
            if (await _customerInfoFileRepository.AnyAsync(c => c.CIFNumber == input.CIFNumber && c.Id != id)  )
            {
                throw new UserFriendlyException(L["CIFNumberMustBeUnique"]);
            }
            var customerInfoFile = await _customerInfoFileManager.UpdateAsync(
            id,
            input.CIFNumber, input.CustomerFirstName, input.CustomerLastName, input.PhoneNumber, input.NationalNumber, input.CustomerAddress, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<CustomerInfoFile, CustomerInfoFileDto>(customerInfoFile);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(CustomerInfoFileExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _customerInfoFileRepository.GetListAsync(input.FilterText, input.CIFNumber, input.CustomerFirstName, input.CustomerLastName, input.PhoneNumber, input.NationalNumber, input.CustomerAddress);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<CustomerInfoFile>, List<CustomerInfoFileExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "CustomerInfoFiles.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new CustomerInfoFileExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new DownloadTokenResultDto
            {
                Token = token
            };
        }
        public async Task<object> GetInfoAsync(string CIFNumber)
        {
            var customer = await _customerInfoFileRepository.FirstOrDefaultAsync(c => c.CIFNumber == CIFNumber);

            if (customer == null)
            {
                throw new UserFriendlyException("Customer not found");
            }

            return new
            {
                CustomerName = $"{customer.CustomerFirstName} {customer.CustomerLastName}",
                NationalNumber = customer.NationalNumber,
                PhoneNumber = customer.PhoneNumber,
                CustomerAddress = new
                {
                    Address = customer.CustomerAddress 
                }
            };
        }

    }
}