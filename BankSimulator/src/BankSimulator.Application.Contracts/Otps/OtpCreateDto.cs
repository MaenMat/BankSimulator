using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BankSimulator.Otps
{
    public class OtpCreateDto
    {
        public string? TransactionNumber { get; set; }
        public string? Code { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}