using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class PayrollLineConfiguration : IEntityTypeConfiguration<PayrollLine>
    {
        public void Configure(EntityTypeBuilder<PayrollLine> builder)
        {
            builder.ToTable("payroll_lines");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.ProjectName).HasMaxLength(200).IsRequired();
            builder.Property(p => p.Hours).HasColumnType("decimal(10,2)");
            builder.Property(p => p.HourlyRate).HasColumnType("decimal(10,2)");
            builder.Property(p => p.Amount).HasColumnType("decimal(12,2)");
        }
    }
}