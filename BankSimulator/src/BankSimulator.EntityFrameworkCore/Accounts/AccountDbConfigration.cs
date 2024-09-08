using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace BankSimulator.Accounts
{
    internal class AccountDbConfigration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            //make a tabel 
            builder.ToTable(BankSimulatorConsts.DbTablePrefix + "Accounts", BankSimulatorConsts.DbSchema);
            //fill the full audit aggregate root
            builder.ConfigureByConvention();

            builder.Property(x => x.AccountNumber).HasColumnName(nameof(Account.AccountNumber));
            builder.Property(x => x.Balance).HasColumnName(nameof(Account.Balance));


            builder.HasMany(x => x.CustomerInfoFiles).WithOne().HasForeignKey(x => x.AccountId).IsRequired().OnDelete(DeleteBehavior.NoAction);

        }
    }
}
