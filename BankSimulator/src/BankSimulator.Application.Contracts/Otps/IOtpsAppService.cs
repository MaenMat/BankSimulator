using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using BankSimulator.Shared;

namespace BankSimulator.Otps
{
    public interface IOtpsAppService : IApplicationService
    {
        Task<PagedResultDto<OtpDto>> GetListAsync(GetOtpsInput input);

        Task<OtpDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<OtpDto> CreateAsync(OtpCreateDto input);

        Task<OtpDto> UpdateAsync(Guid id, OtpUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(OtpExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}