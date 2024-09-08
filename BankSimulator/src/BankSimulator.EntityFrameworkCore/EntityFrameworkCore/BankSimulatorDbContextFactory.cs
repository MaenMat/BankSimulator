using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BankSimulator.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class BankSimulatorDbContextFactory : IDesignTimeDbContextFactory<BankSimulatorDbContext>
{
    public BankSimulatorDbContext CreateDbContext(string[] args)
    {
        BankSimulatorEfCoreEntityExtensionMappings.Configure();

        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<BankSimulatorDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));

        return new BankSimulatorDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../BankSimulator.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
