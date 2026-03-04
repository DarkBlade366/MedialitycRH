using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Payments.Aguinaldo.DTOs
{
    public class AguinaldoPaymentResponse
    {
        public Guid Id { get; set; }
        public Guid PayrollId { get; set; }
        public Guid AguinaldoRuleId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; }
    }
}
