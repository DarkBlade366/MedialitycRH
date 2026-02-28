using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Application.Features.Employees.Commands;

namespace Application.Features.Employees.Validations
{
    public class ChangeEmployeeStatusValidation : AbstractValidator<ChangeEmployeeStatusCommand>
    {
        public ChangeEmployeeStatusValidation()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Employee Id is required.");
        }
    }
}
