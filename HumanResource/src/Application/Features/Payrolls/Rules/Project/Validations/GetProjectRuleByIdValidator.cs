using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Project.Queries;
using FluentValidation;

namespace Application.Features.Payrolls.Rules.Project.Validations
{
    public class GetProjectRuleByIdValidator : AbstractValidator<GetProjectRuleByIdQuery>
    {
        public GetProjectRuleByIdValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required.");
        }
    }
}