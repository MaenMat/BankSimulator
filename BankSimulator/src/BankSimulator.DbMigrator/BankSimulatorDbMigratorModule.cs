using BankSimulator.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace BankSimulator.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(BankSimulatorEntityFrameworkCoreModule),
    typeof(BankSimulatorApplicationContractsModule)
)]
public class BankSimulatorDbMigratorModule : AbpModule
{
}
