using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace BankSimulator.Otps
{
    public class OtpManager : DomainService
    {
        private readonly IOtpRepository _otpRepository;

        public OtpManager(IOtpRepository otpRepository)
        {
            _otpRepository = otpRepository;
        }

        public async Task<Otp> CreateAsync(
        string transactionNumber, string code, DateTime? expiryDate = null)
        {

            var otp = new Otp(
             GuidGenerator.Create(),
             transactionNumber, code, expiryDate
             );

            return await _otpRepository.InsertAsync(otp);
        }

        public async Task<Otp> UpdateAsync(
            Guid id,
            string code, DateTime? expiryDate = null, [CanBeNull] string concurrencyStamp = null
        )
        {

            var otp = await _otpRepository.GetAsync(id);

            otp.Code = code;
            otp.ExpiryDate = expiryDate;

            otp.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _otpRepository.UpdateAsync(otp);
        }

    }
}