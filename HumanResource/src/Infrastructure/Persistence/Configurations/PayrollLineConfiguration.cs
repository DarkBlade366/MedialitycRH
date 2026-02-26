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
            builder.HasKey(pl => pl.Id);
            builder.Property(pl => pl.ProjectId).IsRequired();
            builder.Property(pl => pl.ProjectName).IsRequired().HasMaxLength(200);
            builder.Property(pl => pl.Hours).IsRequired().HasColumnType("decimal(10,2)");
            builder.Property(pl => pl.HourlyRate).IsRequired().HasColumnType("decimal(10,2)");
            builder.Property(pl => pl.Amount).IsRequired().HasColumnType("decimal(12,2)");
            builder.Property(pl => pl.CreatedAt).IsRequired();
            builder.Property(pl => pl.UpdatedAt);

            builder.HasOne(pl => pl.Payroll)
                .WithMany(p => p.Lines)
                .HasForeignKey(pl => pl.PayrollId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}