using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Payments.Project.DTOs;
using Application.Features.Payrolls.Payments.Project.Handlers;
using Application.Features.Payrolls.Payments.Project.Queries;
using Application.Features.Payrolls.Payments.Project.Validations;
using FastEndpoints;
using FluentValidation;

namespace Web.API.Endpoints.Payrolls.Payments.Project
{
    public class GetProjectPaymentsPagedEndpoint : Endpoint<GetProjectPaymentsPagedQuery, PagedResponse<ProjectPaymentResponse>>
    {
        private readonly GetProjectPaymentsPagedHandler _handler;

        public GetProjectPaymentsPagedEndpoint(GetProjectPaymentsPagedHandler handler)
        {
            _handler = handler;
        }
        public override void Configure()
        {
            Get("/payrolls/payments/project-payment");
            Roles("Administrator", "HumanResources", "ProjectManager");
            Validator<GetProjectPaymentsPagedValidator>();
            Summary(s =>
            {
                s.Summary = "Get a paginated list of project payment records.";
                s.Description = "Retrieve historical project payments with optional filtering by payroll, project, and date range.";
                s.ExampleRequest = new GetProjectPaymentsPagedQuery
                {
                    Page = 1,
                    PageSize = 10
                };
            });
        }

        public override async Task HandleAsync(GetProjectPaymentsPagedQuery req, CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);
            await Send.OkAsync(result, ct);
        }
    }
}
