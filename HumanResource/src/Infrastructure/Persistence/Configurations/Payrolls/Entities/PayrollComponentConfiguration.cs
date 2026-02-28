using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Entities;
using Domain.Features.Payrolls.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Payrolls.Entities
{
    public class PayrollComponentConfiguration : IEntityTypeConfiguration<PayrollComponent>
    {
        public void Configure(EntityTypeBuilder<PayrollComponent> builder)
        {
            builder.ToTable("payroll_components");
            builder.HasKey(pc => pc.Id);
            builder.Property(pc => pc.Type).IsRequired().HasConversion<string>();
            builder.Property(pc => pc.Category).IsRequired().HasConversion<string>();
            builder.Property(pc => pc.Description).IsRequired().HasMaxLength(200);
            builder.Property(pc => pc.Amount).IsRequired().HasPrecision(18, 2);
        }
    }
}
