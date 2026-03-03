using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Payroll.Commands;
using FluentValidation;

namespace Application.Features.Payrolls.Payroll.Validations
{
    public class CreatePayrollCommandValidator : AbstractValidator<CreatePayrollCommand>
    {
        public CreatePayrollCommandValidator()
        {
            RuleFor(x => x.employeeId)
                .NotEmpty()
                .WithMessage("EmployeeId is required.");
    
            RuleFor(x => x.periodStart)
                .NotEmpty()
                .WithMessage("PeriodStart is required.");
    
            RuleFor(x => x.periodEnd)
                .NotEmpty()
                .WithMessage("PeriodEnd is required.");
    
            RuleFor(x => x)
                .Must(x => x.periodStart < x.periodEnd)
                .WithMessage("PeriodStart must be earlier than PeriodEnd.");
        }
    }
}