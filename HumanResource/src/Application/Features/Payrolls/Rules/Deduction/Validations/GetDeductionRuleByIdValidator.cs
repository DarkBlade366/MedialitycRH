using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Deduction.Queries;
using FluentValidation;

namespace Application.Features.Payrolls.Rules.Deduction.Validations
{
    public class GetDeductionRuleByIdValidator : AbstractValidator<GetDeductionRuleByIdQuery>
    {
        public GetDeductionRuleByIdValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El ID es requerido.");
        }
    }
}