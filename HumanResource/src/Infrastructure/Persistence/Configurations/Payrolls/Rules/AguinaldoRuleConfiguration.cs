using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Payrolls.Rules
{
    public class AguinaldoRuleConfiguration : IEntityTypeConfiguration<AguinaldoRule>
    {
        public void Configure(EntityTypeBuilder<AguinaldoRule> builder)
        {
            builder.ToTable("aguinaldo_rules");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.MonthlyAccrualPercentage).IsRequired().HasPrecision(5, 4);
            builder.Property(r => r.PayMonth).IsRequired();
            builder.Property(r => r.IsActive).IsRequired();
            builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
        }
    }
}
