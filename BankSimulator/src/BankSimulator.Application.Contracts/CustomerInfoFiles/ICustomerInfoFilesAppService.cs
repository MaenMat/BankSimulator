using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using BankSimulator.Shared;

namespace BankSimulator.CustomerInfoFiles
{
    public interface ICustomerInfoFilesAppService : IApplicationService
    {
        Task<PagedResultDto<CustomerInfoFileDto>> GetListAsync(GetCustomerInfoFilesInput input);

        Task<CustomerInfoFileDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<CustomerInfoFileDto> CreateAsync(CustomerInfoFileCreateDto input);

        Task<CustomerInfoFileDto> UpdateAsync(Guid id, CustomerInfoFileUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(CustomerInfoFileExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
        Task<object> GetInfoAsync(string CIFNumber);
    }
}