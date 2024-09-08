using BankSimulator.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace BankSimulator;

[DependsOn(
    typeof(BankSimulatorEntityFrameworkCoreTestModule)
    )]
public class BankSimulatorDomainTestModule : AbpModule
{

}
