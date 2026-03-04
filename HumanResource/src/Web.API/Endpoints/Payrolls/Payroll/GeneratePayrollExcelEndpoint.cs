using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Payroll.Commands;
using Application.Features.Payrolls.Payroll.Handlers;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Payroll
{
    public class GeneratePayrollExcelEndpoint: Endpoint<GeneratePayrollPdfCommand>
    {
        private readonly GeneratePayrollExcelHandler _handler;

        public GeneratePayrollExcelEndpoint(GeneratePayrollExcelHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/payrolls/{id}/excel");
            Roles("Administrator, HumanResourse");
        }
    
        public override async Task HandleAsync(GeneratePayrollPdfCommand req, CancellationToken ct)
        {
            var excelBytes = await _handler.Handle(req, ct);

            await Send.BytesAsync(
                excelBytes,
                $"payroll-{req.Id}.xlsx",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}