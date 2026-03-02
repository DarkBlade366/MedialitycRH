using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Deduction.Commands;
using FluentValidation;

namespace Application.Features.Payrolls.Rules.Deduction.Validations
{
    public class CreateDeductionRuleValidator : AbstractValidator<CreateDeductionRuleCommand>
    {
        public CreateDeductionRuleValidator()
        {
            
        }
    }
}