using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Aguinaldo.Commands;
using FluentValidation;

namespace Application.Features.Payrolls.Rules.Aguinaldo.Validations
{
    public class ChangeAguinaldoRuleStatusValidator : AbstractValidator<ChangeAguinaldoRuleStatusCommand>
    {
        public ChangeAguinaldoRuleStatusValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Aguinaldo Id is required.");
        }
    }
}