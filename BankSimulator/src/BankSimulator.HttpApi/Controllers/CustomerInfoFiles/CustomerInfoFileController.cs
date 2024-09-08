using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using BankSimulator.CustomerInfoFiles;
using Volo.Abp.Content;
using BankSimulator.Shared;

namespace BankSimulator.Controllers.CustomerInfoFiles
{
    [RemoteService]
    [Area("app")]
    [ControllerName("CustomerInfoFile")]
    [Route("api/app/customer-info-files")]

    public class CustomerInfoFileController : AbpController, ICustomerInfoFilesAppService
    {
        private readonly ICustomerInfoFilesAppService _customerInfoFilesAppService;

        public CustomerInfoFileController(ICustomerInfoFilesAppService customerInfoFilesAppService)
        {
            _customerInfoFilesAppService = customerInfoFilesAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<CustomerInfoFileDto>> GetListAsync(GetCustomerInfoFilesInput input)
        {
            return _customerInfoFilesAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<CustomerInfoFileDto> GetAsync(Guid id)
        {
            return _customerInfoFilesAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<CustomerInfoFileDto> CreateAsync(CustomerInfoFileCreateDto input)
        {
            return _customerInfoFilesAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<CustomerInfoFileDto> UpdateAsync(Guid id, CustomerInfoFileUpdateDto input)
        {
            return _customerInfoFilesAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _customerInfoFilesAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(CustomerInfoFileExcelDownloadDto input)
        {
            return _customerInfoFilesAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _customerInfoFilesAppService.GetDownloadTokenAsync();
        }
        [HttpGet]
        [Route("get-info")]
        public Task<object> GetInfoAsync(string CIFNumber)
        {
            return _customerInfoFilesAppService.GetInfoAsync(CIFNumber);
        }
    }
}