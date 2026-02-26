using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Application.Payrolls.Commands;
using Application.Payrolls.DTOs;
using Application.Payrolls.Handlers;
using Application.Payrolls.Validations;

namespace Web.API.Endpoints.Payrolls
{
    public class GeneratePayrollEndpoint : Endpoint<GeneratePayrollCommand, GeneratePayrollResponse>
    {
        private readonly GeneratePayrollHandler _handler;

        public GeneratePayrollEndpoint(GeneratePayrollHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Post("/payrolls/generate");
            Roles("Administrator", "HumanResources");
            Validator<GeneratePayrollValidator>();
            Summary(s =>
            {
                s.Summary = "Generate payroll for employee in given period";
                s.Description = "Generates a payroll including base salary, project bonus and overtime for the specified employee and date range (UTC required).";
                s.ExampleRequest = new GeneratePayrollCommand
                {
                    From = DateTime.UtcNow.AddMonths(-1),
                    To = DateTime.UtcNow
                };
            });
        }

        public override async Task HandleAsync(GeneratePayrollCommand req, CancellationToken ct)
        {
            var id = await _handler.Handle(req);

            await Send.OkAsync(new GeneratePayrollResponse
            {
                PayrollId = id
            }, ct);
        }
    }
}