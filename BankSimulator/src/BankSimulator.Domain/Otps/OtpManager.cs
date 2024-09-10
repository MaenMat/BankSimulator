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
        string transactionNumber, DateTime? expiryDate = null)
        {
            var code = GenerateRandomCharAndNumberString();
            var otp = new Otp(
             GuidGenerator.Create(),
             transactionNumber, code , expiryDate
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

        public static string GenerateRandomCharAndNumberString()
        {
            var random = new Random();

            // Generate 4 random letters
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string randomChars = new string(Enumerable.Repeat(chars, 4)
                                            .Select(s => s[random.Next(s.Length)]).ToArray());

            // Generate 4 random digits
            string randomNumbers = random.Next(1000, 9999).ToString();

            // Combine them
            return randomChars + randomNumbers;
        }


    }
}