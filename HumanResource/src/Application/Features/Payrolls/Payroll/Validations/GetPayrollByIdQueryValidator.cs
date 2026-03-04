using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Payroll.Queries;
using FluentValidation;

namespace Application.Features.Payrolls.Payroll.Validations
{
    public class GetPayrollByIdQueryValidator : AbstractValidator<GetPayrollByIdQuery>
    {
        public GetPayrollByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El ID es requerido.");
        }
    }
}