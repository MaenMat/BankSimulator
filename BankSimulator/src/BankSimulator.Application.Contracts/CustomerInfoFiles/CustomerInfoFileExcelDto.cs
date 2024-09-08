using System;

namespace BankSimulator.CustomerInfoFiles
{
    public class CustomerInfoFileExcelDto
    {
        public string? CIFNumber { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string PhoneNumber { get; set; }
        public string NationalNumber { get; set; }
        public string? CustomerAddress { get; set; }
    }
}