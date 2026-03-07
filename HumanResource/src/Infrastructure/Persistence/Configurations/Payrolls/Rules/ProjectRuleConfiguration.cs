using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Payrolls.Rules
{
    public class ProjectRuleConfiguration : IEntityTypeConfiguration<ProjectRule>
    {
        public void Configure(EntityTypeBuilder<ProjectRule> builder)
        {
            builder.ToTable("project_rules");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.RedmineProjectId).IsRequired();
            builder.Property(r => r.BonusAmount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
            builder.Property(r => r.IsActive).IsRequired();
        }
    }
}