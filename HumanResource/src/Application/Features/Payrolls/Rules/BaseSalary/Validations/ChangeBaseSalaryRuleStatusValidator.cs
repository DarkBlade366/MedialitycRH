using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.BaseSalary.Commands;
using FluentValidation;

namespace Application.Features.Payrolls.Rules.BaseSalary.Validations
{
    public class ChangeBaseSalaryRuleStatusValidator : AbstractValidator<ChangeBaseSalaryRuleStatusCommand>
    {
        public ChangeBaseSalaryRuleStatusValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("BaseSalary Id is required.");
        }
    }
}