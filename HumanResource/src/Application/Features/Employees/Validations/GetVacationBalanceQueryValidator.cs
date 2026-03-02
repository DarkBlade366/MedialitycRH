using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Employees.Queries;
using FluentValidation;

namespace Application.Features.Employees.Validations
{
    public class GetVacationBalanceQueryValidator : AbstractValidator<GetVacationBalanceQuery>
    {
        public GetVacationBalanceQueryValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty()
                .WithMessage("EmployeeId is required.");
        }
    }
}