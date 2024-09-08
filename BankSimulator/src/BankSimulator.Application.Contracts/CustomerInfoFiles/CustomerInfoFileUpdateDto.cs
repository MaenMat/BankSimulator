using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace BankSimulator.CustomerInfoFiles
{
    public class CustomerInfoFileUpdateDto : IHasConcurrencyStamp
    {
        public string? CIFNumber { get; set; }
        [Required]
        public string CustomerFirstName { get; set; }
        [Required]
        public string CustomerLastName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string NationalNumber { get; set; }
        public string? CustomerAddress { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}