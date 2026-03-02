using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Vacation.Commands;
using FluentValidation;

namespace Application.Features.Payrolls.Rules.Vacation.Validations
{
    public class CreateVacationRuleValidator : AbstractValidator<CreateVacationRuleCommand>
    { 
        public CreateVacationRuleValidator()
        {
            
        }
    }
}