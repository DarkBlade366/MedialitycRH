using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Payroll.Queries;
using FluentValidation;

namespace Application.Features.Payrolls.Payroll.Validations
{
    public class GetPayrollPagedQueryValidator : AbstractValidator<GetPayrollPagedQuery>
    {
        public GetPayrollPagedQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
        }
    }
}