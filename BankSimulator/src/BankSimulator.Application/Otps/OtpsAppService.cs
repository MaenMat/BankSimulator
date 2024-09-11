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
using BankSimulator.Otps;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using BankSimulator.Shared;
using BankSimulator.Transactions;

namespace BankSimulator.Otps
{
    [RemoteService(IsEnabled = false)]
    [Authorize(BankSimulatorPermissions.Otps.Default)]
    public class OtpsAppService : ApplicationService, IOtpsAppService
    {
        private readonly IDistributedCache<OtpExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly IOtpRepository _otpRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly OtpManager _otpManager;

        public OtpsAppService(IOtpRepository otpRepository, OtpManager otpManager, IDistributedCache<OtpExcelDownloadTokenCacheItem, string> excelDownloadTokenCache, ITransactionRepository transactionRepository)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _otpRepository = otpRepository;
            _otpManager = otpManager;
            _transactionRepository = transactionRepository;
        }

        public virtual async Task<PagedResultDto<OtpDto>> GetListAsync(GetOtpsInput input)
        {
            var totalCount = await _otpRepository.GetCountAsync(input.FilterText, input.TransactionNumber, input.Code, input.ExpiryDateMin, input.ExpiryDateMax);
            var items = await _otpRepository.GetListAsync(input.FilterText, input.TransactionNumber, input.Code, input.ExpiryDateMin, input.ExpiryDateMax, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<OtpDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Otp>, List<OtpDto>>(items)
            };
        }

        public virtual async Task<OtpDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Otp, OtpDto>(await _otpRepository.GetAsync(id));
        }

        [Authorize(BankSimulatorPermissions.Otps.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _otpRepository.DeleteAsync(id);
        }

        [Authorize(BankSimulatorPermissions.Otps.Create)]
        public virtual async Task<OtpDto> CreateAsync(OtpCreateDto input)
        {
            if(input.TransactionNumber == null)
            {
                throw new UserFriendlyException(L["TransactionNumberIsRequired"]);
            }

            var otp = await _otpManager.CreateAsync(
            input.TransactionNumber, input.ExpiryDate
            );

            return ObjectMapper.Map<Otp, OtpDto>(otp);
        }

        [Authorize(BankSimulatorPermissions.Otps.Edit)]
        public virtual async Task<OtpDto> UpdateAsync(Guid id, OtpUpdateDto input)
        {

            var otp = await _otpManager.UpdateAsync(
            id,
            input.Code, input.ExpiryDate, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Otp, OtpDto>(otp);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(OtpExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _otpRepository.GetListAsync(input.FilterText, input.TransactionNumber, input.Code, input.ExpiryDateMin, input.ExpiryDateMax);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<Otp>, List<OtpExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Otps.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new OtpExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new DownloadTokenResultDto
            {
                Token = token
            };
        }
        public virtual async Task<string> ResendOtp(string TransactionNumber)
        {
            var transaction = await _transactionRepository.FirstOrDefaultAsync(t=>t.TransactionNumber == TransactionNumber);
            if (transaction == null)
            {
                throw new UserFriendlyException(L["InvalidTransactionNumber"]);
            }

            if (transaction.TransactionStatus == TransactionStatus.Done)
            {
                throw new UserFriendlyException(L["TransactionStatusIsDone"]);
            }

            var otp = await _otpManager.CreateAsync(
            TransactionNumber, DateTime.Now.AddMinutes(OtpConsts.ExpirationInMinutes)
            );

            return TransactionNumber;
        }
    }
}