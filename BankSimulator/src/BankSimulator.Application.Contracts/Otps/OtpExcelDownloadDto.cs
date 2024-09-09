using Volo.Abp.Application.Dtos;
using System;

namespace BankSimulator.Otps
{
    public class OtpExcelDownloadDto
    {
        public string DownloadToken { get; set; }

        public string? FilterText { get; set; }

        public string? TransactionNumber { get; set; }
        public string? Code { get; set; }
        public DateTime? ExpiryDateMin { get; set; }
        public DateTime? ExpiryDateMax { get; set; }

        public OtpExcelDownloadDto()
        {

        }
    }
}