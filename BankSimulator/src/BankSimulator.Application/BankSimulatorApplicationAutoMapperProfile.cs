using BankSimulator.Otps;
using BankSimulator.Transactions;
using BankSimulator.Accounts;
using System;
using BankSimulator.Shared;
using Volo.Abp.AutoMapper;
using BankSimulator.CustomerInfoFiles;
using AutoMapper;

namespace BankSimulator;

public class BankSimulatorApplicationAutoMapperProfile : Profile
{
    public BankSimulatorApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<CustomerInfoFile, CustomerInfoFileDto>();
        CreateMap<CustomerInfoFile, CustomerInfoFileExcelDto>();

        CreateMap<Account, AccountDto>();
        CreateMap<Account, AccountExcelDto>();

        CreateMap<AccountWithNavigationProperties, AccountWithNavigationPropertiesDto>();
        CreateMap<CustomerInfoFile, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.CIFNumber));

        CreateMap<Transaction, TransactionDto>();
        CreateMap<Transaction, TransactionExcelDto>();
        CreateMap<TransactionWithNavigationProperties, TransactionWithNavigationPropertiesDto>();
        CreateMap<Account, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.AccountNumber));
        CreateMap<Account, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.AccountNumber));

        CreateMap<Otp, OtpDto>();
        CreateMap<Otp, OtpExcelDto>();
    }
}