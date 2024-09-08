using System.Threading.Tasks;

namespace BankSimulator.Data;

public interface IBankSimulatorDbSchemaMigrator
{
    Task MigrateAsync();
}
