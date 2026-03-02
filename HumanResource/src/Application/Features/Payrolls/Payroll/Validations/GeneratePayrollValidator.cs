using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Application.Features.Payrolls.Payroll.Commands;

namespace Application.Features.Payrolls.Payroll.Validations
{
    public class GeneratePayrollValidator
        : AbstractValidator<GeneratePayrollCommand>
    {
        public GeneratePayrollValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty()
                .WithMessage("EmployeeId is required.");

            RuleFor(x => x.From)
                .NotEmpty()
                .WithMessage("From date is required.");

            RuleFor(x => x.To)
                .NotEmpty()
                .WithMessage("To date is required.");

            RuleFor(x => x)
                .Must(x => x.From <= x.To)
                .WithMessage("From date must be less than or equal to To date.");
        }
    }
}
