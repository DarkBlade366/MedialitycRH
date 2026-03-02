using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Overtime.Commands;
using FluentValidation;

namespace Application.Features.Payrolls.Rules.Overtime.Validations
{
    public class CreateOvertimeRuleValidator : AbstractValidator<CreateOvertimeRuleCommand>
    {
        public CreateOvertimeRuleValidator()
        {
            RuleFor(x => x.StandardHoursPerPeriod)
                .GreaterThan(0).WithMessage("Standard hours per period must be greater than 0.");

            RuleFor(x => x.OvertimeMultiplier)
                .GreaterThan(1).WithMessage("Overtime multiplier must be greater than 1.");
        }
    }
}