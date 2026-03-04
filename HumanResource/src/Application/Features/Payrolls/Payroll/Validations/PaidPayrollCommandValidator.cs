using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Payroll.Commands;
using FluentValidation;

namespace Application.Features.Payrolls.Payroll.Validations
{
    public class PaidPayrollCommandValidator : AbstractValidator<PaidPayrollCommand>
    {
        public PaidPayrollCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Payroll Id is required.");
        }
    }
}