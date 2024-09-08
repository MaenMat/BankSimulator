using System;

namespace BankSimulator.CustomerInfoFiles;

[Serializable]
public class CustomerInfoFileExcelDownloadTokenCacheItem
{
    public string Token { get; set; }
}