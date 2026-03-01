using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Aguinaldo.Queries;
using FluentValidation;

namespace Application.Features.Payrolls.Rules.Aguinaldo.Validations
{
    public class GetAguinaldoByIdValidator : AbstractValidator<GetAguinaldoRuleByIdQuery>
    {
        public GetAguinaldoByIdValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El ID es requerido.");
        }
    }
}