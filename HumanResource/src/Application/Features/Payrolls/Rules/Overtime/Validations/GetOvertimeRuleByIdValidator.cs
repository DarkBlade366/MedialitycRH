using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Overtime.Queries;
using FluentValidation;

namespace Application.Features.Payrolls.Rules.Overtime.Validations
{
    public class GetOvertimeRuleByIdValidator : AbstractValidator<GetOvertimeRuleByIdQuery>
    {
        public GetOvertimeRuleByIdValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El ID es requerido.");
        }
    }
}