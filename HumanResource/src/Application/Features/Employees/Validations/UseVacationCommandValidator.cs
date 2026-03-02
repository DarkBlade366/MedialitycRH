using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Employees.Commands;
using FluentValidation;

namespace Application.Features.Employees.Validations
{
    public class UseVacationCommandValidator: AbstractValidator<UseVacationCommand>
    {
        public UseVacationCommandValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty()
                .WithMessage("EmployeeId is required.");

            RuleFor(x => x.Days)
                .GreaterThan(0)
                .WithMessage("Vacation days must be greater than zero.");
        }
    }
}