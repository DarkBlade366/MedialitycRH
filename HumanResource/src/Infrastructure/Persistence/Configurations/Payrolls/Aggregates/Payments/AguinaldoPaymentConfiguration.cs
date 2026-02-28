using Domain.Features.Payrolls.Aggregates.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Payrolls.Aggregates.Payments
{
    public class AguinaldoPaymentConfiguration : IEntityTypeConfiguration<AguinaldoPayment>
    {
        public void Configure(EntityTypeBuilder<AguinaldoPayment> builder)
        {
            builder.ToTable("aguinaldo_payments");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Amount).IsRequired();
            builder.Property(p => p.PaidAt).IsRequired();
            builder.Property(p => p.PayrollId).IsRequired();
            builder.Property(p => p.AguinaldoRuleId).IsRequired();
        }
    }
}