using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Payroll.Commands;
using Application.Features.Payrolls.Payroll.Handlers;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Payroll
{
    public class GeneratePayrollPdfEndpoint : Endpoint<GeneratePayrollPdfCommand>
    {
        private readonly GeneratePayrollPdfHandler _handler;

        public GeneratePayrollPdfEndpoint(GeneratePayrollPdfHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/payrolls/{id}/pdf");
            Roles("Administrator, HumanResourse");
            Summary(s =>
            {
                s.Summary = "Generate payroll PDF.";
                s.Description = "Generates a PDF receipt for the specified payroll.";
            });
        }

        public override async Task HandleAsync(GeneratePayrollPdfCommand req, CancellationToken ct)
        {
            var pdfBytes = await _handler.Handle(req, ct);

            await Send.BytesAsync(
                pdfBytes,
                $"payroll-{req}.pdf",
                "application/pdf");
        }
    }
}