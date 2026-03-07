using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Application.Features.Analytics.Queries;

namespace Application.Features.Analytics.Validations
{
    public class GetHoursComparisonValidator : AbstractValidator<GetHoursComparisonQuery>
    {
        public GetHoursComparisonValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty()
                .WithMessage("Employee ID is required when specified.");
                
            RuleFor(x => x.PeriodStart)
                .NotEmpty()
                .WithMessage("Start period date is required.");

            RuleFor(x => x.PeriodEnd)
                .NotEmpty()
                .WithMessage("End period date is required.")
                .GreaterThanOrEqualTo(x => x.PeriodStart)
                .WithMessage("End period date must be greater than or equal to start period date.");
        }
    }
}