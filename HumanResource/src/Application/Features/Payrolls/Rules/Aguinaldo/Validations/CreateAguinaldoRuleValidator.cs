using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Aguinaldo.Commands;
using FluentValidation;

namespace Application.Features.Payrolls.Rules.Aguinaldo.Validations
{
    public class CreateAguinaldoRuleValidator : AbstractValidator<CreateAguinaldoRuleCommand>
    {
        public CreateAguinaldoRuleValidator()
        {
            RuleFor(x => x.PayMonth)
                .GreaterThan(0).WithMessage("Los meses de pago deben ser mayores a 0.")
                .LessThanOrEqualTo(12).WithMessage("Los meses de pago no pueden ser mayores a 12.");

            RuleFor(x => x.MonthlyAccrualPercentage)
                .GreaterThan(0).WithMessage("El porcentaje debe ser mayor a 0.")
                .LessThanOrEqualTo(100).WithMessage("El porcentaje no puede ser mayor a 100.");
        }
    }
}