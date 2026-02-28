using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Payrolls.Rules
{
    public class MilestoneRuleConfiguration : IEntityTypeConfiguration<MilestoneRule>
    {
        public void Configure(EntityTypeBuilder<MilestoneRule> builder)
        {
            builder.ToTable("milestone_rules");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.RedmineProjectId).IsRequired();
            builder.Property(r => r.MilestoneName).IsRequired().HasMaxLength(100);
            builder.Property(r => r.BonusAmount).IsRequired().HasPrecision(18, 2);
            builder.Property(r => r.IsActive).IsRequired();
            builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
        }
    }
}
