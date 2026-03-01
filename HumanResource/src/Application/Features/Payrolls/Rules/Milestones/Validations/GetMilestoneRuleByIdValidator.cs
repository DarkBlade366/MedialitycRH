using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Milestones.Queries;
using FluentValidation;

namespace Application.Features.Payrolls.Rules.Milestones.Validations
{
    public class GetMilestoneRuleByIdValidator : AbstractValidator<GetMilestoneRuleByIdQuery>
    {
        public GetMilestoneRuleByIdValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El ID es requerido.");
        }
    }
}