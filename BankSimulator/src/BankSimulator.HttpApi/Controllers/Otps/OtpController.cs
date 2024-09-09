using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using BankSimulator.Otps;
using Volo.Abp.Content;
using BankSimulator.Shared;

namespace BankSimulator.Controllers.Otps
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Otp")]
    [Route("api/app/otps")]

    public class OtpController : AbpController, IOtpsAppService
    {
        private readonly IOtpsAppService _otpsAppService;

        public OtpController(IOtpsAppService otpsAppService)
        {
            _otpsAppService = otpsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<OtpDto>> GetListAsync(GetOtpsInput input)
        {
            return _otpsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<OtpDto> GetAsync(Guid id)
        {
            return _otpsAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<OtpDto> CreateAsync(OtpCreateDto input)
        {
            return _otpsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<OtpDto> UpdateAsync(Guid id, OtpUpdateDto input)
        {
            return _otpsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _otpsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(OtpExcelDownloadDto input)
        {
            return _otpsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _otpsAppService.GetDownloadTokenAsync();
        }
    }
}