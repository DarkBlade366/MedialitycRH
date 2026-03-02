using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Overtime.Commands;
using FluentValidation;

namespace Application.Features.Payrolls.Rules.Overtime.Validations
{
    public class ChangeOvertimeRuleStatusValidator : AbstractValidator<ChangeOvertimeRuleStatusCommand>
    {
        public ChangeOvertimeRuleStatusValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Employee Id is required.");
        }
    }
}