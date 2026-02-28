using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Payrolls.Rules
{
    public class OvertimeRuleConfiguration : IEntityTypeConfiguration<OvertimeRule>
    {
        public void Configure(EntityTypeBuilder<OvertimeRule> builder)
        {
            builder.ToTable("overtime_rules");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.StandardHoursPerPeriod).IsRequired();
            builder.Property(r => r.OvertimeMultiplier).IsRequired().HasPrecision(5, 2);
            builder.Property(r => r.IsActive).IsRequired();
            builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
        }
    }
}
