using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Application.Features.Analytics.Queries;

namespace Application.Features.Analytics.Validations
{
    public class GetProductivityTrendValidator : AbstractValidator<GetProductivityTrendQuery>
    {
        public GetProductivityTrendValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty()
                .WithMessage("Employee ID is required.");
                
            RuleFor(x => x.From)
                .NotEmpty()
                .WithMessage("From date is required.");

            RuleFor(x => x.To)
                .NotEmpty()
                .WithMessage("To date is required.")
                .GreaterThanOrEqualTo(x => x.From)
                .WithMessage("To date must be after or equal to From date.");
        }
    }
}