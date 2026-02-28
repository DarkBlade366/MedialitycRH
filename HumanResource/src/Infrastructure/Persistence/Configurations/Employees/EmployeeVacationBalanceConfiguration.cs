using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Employees.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Employees
{
    public class EmployeeVacationBalanceConfiguration : IEntityTypeConfiguration<EmployeeVacationBalance>
    {
        public void Configure(EntityTypeBuilder<EmployeeVacationBalance> builder)
        {
            builder.ToTable("employee_vacation_balances");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.EmployeeId).IsRequired();
            builder.HasIndex(b => b.EmployeeId).IsUnique();
            builder.Property(b => b.AccruedDays).IsRequired().HasPrecision(5, 2);
            builder.Property(b => b.UsedDays).IsRequired().HasPrecision(5, 2);
        }
    }
}
