using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class SalaryConfigurationEntity : IEntityTypeConfiguration<SalaryConfiguration>
    {
        public void Configure(EntityTypeBuilder<SalaryConfiguration> builder)
        {
            builder.ToTable("salary_configurations");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Role).HasConversion<string>();
            builder.Property(s => s.BaseHourlyRate).HasColumnType("decimal(10,2)");
            builder.HasIndex(s => s.Role).IsUnique();
        }
    }
}