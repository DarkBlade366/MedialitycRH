using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.BaseSalary.Queries;
using FluentValidation;

namespace Application.Features.Payrolls.Rules.BaseSalary.Validations
{
    public class GetBaseSalaryRuleByIdValidator : AbstractValidator<GetBaseSalaryRuleByIdQuery>
    {
        public GetBaseSalaryRuleByIdValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El ID es requerido.");
        }
    }
}