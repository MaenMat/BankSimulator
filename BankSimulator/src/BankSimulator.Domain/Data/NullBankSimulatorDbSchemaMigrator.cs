using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace BankSimulator.Data;

/* This is used if database provider does't define
 * IBankSimulatorDbSchemaMigrator implementation.
 */
public class NullBankSimulatorDbSchemaMigrator : IBankSimulatorDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
