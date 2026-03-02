using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Vacation.Commands;
using FluentValidation;

namespace Application.Features.Payrolls.Rules.Vacation.Validations
{
    public class CreateVacationRuleValidator : AbstractValidator<CreateVacationRuleCommand>
    { 
        public CreateVacationRuleValidator()
        {
            RuleFor(x => x.AccrualRatePerMonth)
                .GreaterThan(0).WithMessage("Accrual rate must be greater than 0.")
                .LessThanOrEqualTo(5).WithMessage("Accrual rate cannot exceed 5 days per month.");
        }
    }
}