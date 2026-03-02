using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Vacation.Queries;
using FluentValidation;

namespace Application.Features.Payrolls.Rules.Vacation.Validations
{
    public class GetVacationRuleByIdValidator : AbstractValidator<GetVacationRuleByIdQuery>
    {
        public GetVacationRuleByIdValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El ID es requerido.");
        }
    }
}