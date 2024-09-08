using BankSimulator.Transactions;
using BankSimulator.Accounts;
using BankSimulator.CustomerInfoFiles;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.LanguageManagement.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TextTemplateManagement.EntityFrameworkCore;
using Volo.Saas.EntityFrameworkCore;
using Volo.Saas.Editions;
using Volo.Saas.Tenants;
using Volo.Abp.Gdpr;
using Volo.Abp.OpenIddict.EntityFrameworkCore;

namespace BankSimulator.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityProDbContext))]
[ReplaceDbContext(typeof(ISaasDbContext))]
[ConnectionStringName("Default")]
public class BankSimulatorDbContext :
    AbpDbContext<BankSimulatorDbContext>,
    IIdentityProDbContext,
    ISaasDbContext
{
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<CustomerInfoFile> CustomerInfoFiles { get; set; }
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext and ISaasDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext and ISaasDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; }

    // SaaS
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Edition> Editions { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public BankSimulatorDbContext(DbContextOptions<BankSimulatorDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentityPro();
        builder.ConfigureOpenIddictPro();
        builder.ConfigureFeatureManagement();
        builder.ConfigureLanguageManagement();
        builder.ConfigureSaas();
        builder.ConfigureTextTemplateManagement();
        builder.ConfigureBlobStoring();
        builder.ConfigureGdpr();

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(BankSimulatorConsts.DbTablePrefix + "YourEntities", BankSimulatorConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});
        if (builder.IsHostDatabase())
        {
            builder.Entity<CustomerInfoFile>(b =>
{
    b.ToTable(BankSimulatorConsts.DbTablePrefix + "CustomerInfoFiles", BankSimulatorConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.CIFNumber).HasColumnName(nameof(CustomerInfoFile.CIFNumber));
    b.Property(x => x.CustomerFirstName).HasColumnName(nameof(CustomerInfoFile.CustomerFirstName)).IsRequired();
    b.Property(x => x.CustomerLastName).HasColumnName(nameof(CustomerInfoFile.CustomerLastName)).IsRequired();
    b.Property(x => x.PhoneNumber).HasColumnName(nameof(CustomerInfoFile.PhoneNumber)).IsRequired();
    b.Property(x => x.NationalNumber).HasColumnName(nameof(CustomerInfoFile.NationalNumber)).IsRequired();
    b.Property(x => x.CustomerAddress).HasColumnName(nameof(CustomerInfoFile.CustomerAddress));
});

        }
        if (builder.IsHostDatabase())
        {
            builder.ApplyConfiguration(new AccountDbConfigration());
        }
        if (builder.IsHostDatabase())
        {

            builder.Entity<AccountCustomerInfoFile>(b =>
{
    b.ToTable(BankSimulatorConsts.DbTablePrefix + "AccountCustomerInfoFile" + BankSimulatorConsts.DbSchema);
    b.ConfigureByConvention();

    b.HasKey(
        x => new { x.AccountId, x.CustomerInfoFileId }
    );

    b.HasOne<Account>().WithMany(x => x.CustomerInfoFiles).HasForeignKey(x => x.AccountId).IsRequired().OnDelete(DeleteBehavior.NoAction);
    b.HasOne<CustomerInfoFile>().WithMany().HasForeignKey(x => x.CustomerInfoFileId).IsRequired().OnDelete(DeleteBehavior.NoAction);

    b.HasIndex(
            x => new { x.AccountId, x.CustomerInfoFileId }
    );
});
        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<Transaction>(b =>
{
    b.ToTable(BankSimulatorConsts.DbTablePrefix + "Transactions", BankSimulatorConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.TransactionType).HasColumnName(nameof(Transaction.TransactionType));
    b.Property(x => x.Amount).HasColumnName(nameof(Transaction.Amount));
    b.Property(x => x.Description).HasColumnName(nameof(Transaction.Description));
    b.Property(x => x.TransactionDate).HasColumnName(nameof(Transaction.TransactionDate));
    b.HasOne<Account>().WithMany().HasForeignKey(x => x.SourceAccountId).OnDelete(DeleteBehavior.NoAction);
    b.HasOne<Account>().WithMany().HasForeignKey(x => x.DestinationAccountId).OnDelete(DeleteBehavior.NoAction);
});

        }
    }
}