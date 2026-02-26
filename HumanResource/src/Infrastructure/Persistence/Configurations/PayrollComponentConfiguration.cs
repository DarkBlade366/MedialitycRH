using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class PayrollComponentConfiguration : IEntityTypeConfiguration<PayrollComponent>
    {
        public void Configure(EntityTypeBuilder<PayrollComponent> builder)
        {
            builder.ToTable("payroll_components");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Type).HasConversion<string>();
            builder.Property(c => c.Description).HasMaxLength(300).IsRequired();
            builder.Property(c => c.Amount).HasColumnType("decimal(14,2)");
        }
    }
}