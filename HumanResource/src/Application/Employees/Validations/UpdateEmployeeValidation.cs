using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Application.Employees.Commands;

namespace Application.Employees.Validations
{
    public class UpdateEmployeeValidation : AbstractValidator<UpdateEmployeeCommand>
    {
        public UpdateEmployeeValidation()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Employee Id is required.");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Invalid employee role.");
        }
    }
}