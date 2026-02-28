using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Application.Features.Employees.Commands;
using Application.Features.Employees.Handlers;
using System.Data;

namespace Application.Features.Employees.Validations
{
    public class CreateEmployeeValidation : AbstractValidator<CreateEmployeeCommand>
    {
        public CreateEmployeeValidation()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Invalid employee role.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");

            RuleFor(x => x.RedmineUserId)
                .GreaterThan(0).WithMessage("Redmine User ID must be a positive integer.");
        }
    }
}
