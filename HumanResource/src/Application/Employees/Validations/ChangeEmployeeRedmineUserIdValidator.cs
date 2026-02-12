using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Application.Employees.Commands;

namespace Application.Employees.Validations
{
    public class ChangeEmployeeRedmineUserIdValidator : AbstractValidator<ChangeEmployeeRedmineUserIdCommand>
    {
        public ChangeEmployeeRedmineUserIdValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Employee Id is required.");
            RuleFor(x => x.RedmineUserId)
                .GreaterThan(0).WithMessage("Redmine User Id must be a positive integer.");
        }
    }
}