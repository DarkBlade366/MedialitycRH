using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Application.Features.Employees.Queries;

namespace Application.Features.Employees.Validations
{
    public class GetEmployeeByRedmineUserIdValidation : AbstractValidator<GetEmployeeByRedmineUserIdQuery>
    {
        public GetEmployeeByRedmineUserIdValidation()
        {
            RuleFor(x => x.RedmineUserId)
                .GreaterThan(0).WithMessage("Redmine User ID must be a positive integer.");
        }
    }
}