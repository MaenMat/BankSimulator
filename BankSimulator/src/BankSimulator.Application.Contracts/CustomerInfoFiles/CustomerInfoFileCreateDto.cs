using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BankSimulator.CustomerInfoFiles
{
    public class CustomerInfoFileCreateDto
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
    }
}