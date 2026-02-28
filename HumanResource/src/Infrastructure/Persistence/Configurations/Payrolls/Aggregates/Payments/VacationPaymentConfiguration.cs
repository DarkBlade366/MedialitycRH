using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Features.Payrolls.Aggregates.Payments;

namespace Infrastructure.Persistence.Configurations.Payrolls.Aggregates.Payments
{
    public class VacationPaymentConfiguration : IEntityTypeConfiguration<VacationPayment>
    {
        public void Configure(EntityTypeBuilder<VacationPayment> builder)
        {
            builder.ToTable("vacation_payments");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Amount).IsRequired();
            builder.Property(p => p.PaidAt).IsRequired();
            builder.Property(p => p.PayrollId).IsRequired();
            builder.Property(p => p.VacationRuleId).IsRequired();
        }
    }
}