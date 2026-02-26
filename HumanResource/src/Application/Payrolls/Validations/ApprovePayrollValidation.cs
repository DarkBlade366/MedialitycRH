using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Application.Payrolls.Commands;

namespace Application.Payrolls.Validations
{
    public class ApprovePayrollValidation : AbstractValidator<ApprovePayrollCommand>
    {
        public ApprovePayrollValidation()
        {
            RuleFor(x => x.PayrollId)
                .NotEmpty().WithMessage("Payroll ID is required.");
        }
    }
}