using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Productivity.Queries;
using FluentValidation;

namespace Application.Features.Payrolls.Rules.Productivity.Validations
{
    public class GetProductivityRuleByIdValidator : AbstractValidator<GetProductivityRuleByIdQuery>
    {
        public GetProductivityRuleByIdValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El ID es requerido.");
        }
    }
}