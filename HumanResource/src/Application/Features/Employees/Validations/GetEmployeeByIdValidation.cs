using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Application.Features.Employees.Queries;

namespace Application.Features.Employees.Validations
{
    public class GetEmployeeByIdValidation : AbstractValidator<GetEmployeeByIdQuery>
    {
        public GetEmployeeByIdValidation()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Employee ID is required");
        }
    }
}
