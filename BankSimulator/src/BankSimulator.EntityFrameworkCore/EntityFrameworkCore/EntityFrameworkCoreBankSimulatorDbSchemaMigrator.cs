using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BankSimulator.Data;
using Volo.Abp.DependencyInjection;

namespace BankSimulator.EntityFrameworkCore;

public class EntityFrameworkCoreBankSimulatorDbSchemaMigrator
    : IBankSimulatorDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreBankSimulatorDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the BankSimulatorDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<BankSimulatorDbContext>()
            .Database
            .MigrateAsync();
    }
}
