using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Application.TimeEntries.Queries;

namespace Application.TimeEntries.Validations
{
    public class ListPagedTimeEntriesValidator : AbstractValidator<ListPagedTimeEntriesQuery>
    {
        public ListPagedTimeEntriesValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page must be greater than or equal to 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("PageSize must be between 1 and 100.");

            RuleFor(x => x)
                .Must(x => x.From!.Value <= x.To!.Value)
                .WithMessage("From date must be earlier than or equal to To date.")
                .When(x => x.From.HasValue && x.To.HasValue);
        }
    }
}