using Volo.Abp.Modularity;

namespace BankSimulator;

[DependsOn(
    typeof(BankSimulatorApplicationModule),
    typeof(BankSimulatorDomainTestModule)
    )]
public class BankSimulatorApplicationTestModule : AbpModule
{

}
