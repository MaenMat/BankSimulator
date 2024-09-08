using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace BankSimulator.CustomerInfoFiles
{
    public class CustomerInfoFileDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string? CIFNumber { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string PhoneNumber { get; set; }
        public string NationalNumber { get; set; }
        public string? CustomerAddress { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}