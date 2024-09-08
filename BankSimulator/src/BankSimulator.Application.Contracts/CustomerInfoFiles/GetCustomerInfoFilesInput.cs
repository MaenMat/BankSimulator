using Volo.Abp.Application.Dtos;
using System;

namespace BankSimulator.CustomerInfoFiles
{
    public class GetCustomerInfoFilesInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }

        public string? CIFNumber { get; set; }
        public string? CustomerFirstName { get; set; }
        public string? CustomerLastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? NationalNumber { get; set; }
        public string? CustomerAddress { get; set; }

        public GetCustomerInfoFilesInput()
        {

        }
    }
}