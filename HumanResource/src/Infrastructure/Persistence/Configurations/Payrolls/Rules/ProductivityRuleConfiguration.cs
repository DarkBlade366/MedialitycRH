using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Payrolls.Rules
{
    public class ProductivityRuleConfiguration : IEntityTypeConfiguration<ProductivityRule>
    {
        public void Configure(EntityTypeBuilder<ProductivityRule> builder)
        {
            builder.ToTable("productivity_rules");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.MinimumTarget).IsRequired().HasPrecision(5, 2);
            builder.Property(r => r.BonusValue).IsRequired().HasPrecision(10, 2);
            builder.Property(r => r.BonusType).IsRequired().HasConversion<string>();
            builder.Property(r => r.FullBonusTarget).IsRequired().HasPrecision(10, 2);
            builder.Property(r => r.IsActive).IsRequired();
            builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
        }
    }
}
