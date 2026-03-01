using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Payrolls.Rules
{
    public class BaseSalaryRuleConfiguration : IEntityTypeConfiguration<BaseSalaryRule>
    {
        public void Configure(EntityTypeBuilder<BaseSalaryRule> builder)
        {
            builder.ToTable("base_salary_rules");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Amount).IsRequired().HasPrecision(18, 2);
            builder.Property(r => r.Role).IsRequired().HasConversion<string>();
            builder.Property(r => r.IsActive).IsRequired();
            builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
        }
    }
}
