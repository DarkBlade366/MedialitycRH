using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Payrolls.Rules
{
    public class DeductionRuleConfiguration : IEntityTypeConfiguration<DeductionRule>
    {
        public void Configure(EntityTypeBuilder<DeductionRule> builder)
        {
            builder.ToTable("deduction_rules");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Percentage).IsRequired().HasPrecision(5, 4);
            builder.Property(r => r.IsActive).IsRequired();
            builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
            builder.Property(r => r.Type).IsRequired().HasConversion<string>();
            builder.Property(r => r.IsActive).IsRequired();
        }
    }
}
