using BankSimulator.Otps;
using BankSimulator.Transactions;
using BankSimulator.Accounts;
using Volo.Abp.AutoMapper;
using BankSimulator.CustomerInfoFiles;
using AutoMapper;

namespace BankSimulator.Blazor;

public class BankSimulatorBlazorAutoMapperProfile : Profile
{
    public BankSimulatorBlazorAutoMapperProfile()
    {
        //Define your AutoMapper configuration here for the Blazor project.

        CreateMap<CustomerInfoFileDto, CustomerInfoFileUpdateDto>();

        CreateMap<AccountDto, AccountUpdateDto>();

        CreateMap<AccountDto, AccountUpdateDto>().Ignore(x => x.CustomerInfoFileIds);

        CreateMap<TransactionDto, TransactionUpdateDto>();

        CreateMap<OtpDto, OtpUpdateDto>();
    }
}