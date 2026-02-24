using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Application.TimeEntries.Queries;

namespace Application.TimeEntries.Validations
{
    public class ListTimeEntriesQueryValidator : AbstractValidator<ListTimeEntriesQuery>
    {
        public ListTimeEntriesQueryValidator()
        {
            RuleFor(x => x.EmployeeId).NotEmpty()
                .WithMessage("EmployeeId is required.");

            RuleFor(x => x.From).LessThanOrEqualTo(x => x.To)
                .WithMessage("From date must be less than or equal to To date.");
        }
    }
}