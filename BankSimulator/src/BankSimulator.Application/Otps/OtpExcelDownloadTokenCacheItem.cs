using System;

namespace BankSimulator.Otps;

[Serializable]
public class OtpExcelDownloadTokenCacheItem
{
    public string Token { get; set; }
}