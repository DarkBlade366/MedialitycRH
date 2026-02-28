using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Employees.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Employees
{
    public class EmployeeAguinaldoBalanceConfiguration : IEntityTypeConfiguration<EmployeeAguinaldoBalance>
    {
        public void Configure(EntityTypeBuilder<EmployeeAguinaldoBalance> builder)
        {
            builder.ToTable("employee_aguinaldo_balances");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.EmployeeId).IsRequired();
            builder.HasIndex(b => b.EmployeeId).IsUnique();
            builder.Property(b => b.AccruedAmount).IsRequired().HasPrecision(18, 2);
            builder.Property(b => b.PaidAmount).IsRequired().HasPrecision(18, 2);
        }
    }
}
