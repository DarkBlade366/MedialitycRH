using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.BaseSalary.Commands;
using FluentValidation;

namespace Application.Features.Payrolls.Rules.BaseSalary.Validations
{
    public class CreateBaseSalaryRuleValidator : AbstractValidator<CreateBaseSalaryRuleCommand>
    {
        public CreateBaseSalaryRuleValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("BaseSalary must be greater than 0.");
    
            RuleFor(x => x.Role)
                .NotEmpty().WithMessage(".");;
        }
    }
}