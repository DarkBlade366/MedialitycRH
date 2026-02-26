using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Domain.Models;
using Application.Payrolls.Interfaces;

namespace Infrastructure.Reports
{
    public class PayrollPdfGenerator : IPayrollPdfGenerator
    {
        public byte[] Generate(Payroll payroll, string employeeName)
        {
            QuestPDF.Settings.License = LicenseType.Community;
    
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
    
                    page.Header()
                        .Text("Payroll Receipt")
                        .FontSize(20)
                        .SemiBold()
                        .AlignCenter();
    
                    page.Content().Column(column =>
                    {
                        column.Spacing(10);
    
                        column.Item().Text($"Employee: {employeeName}");
                        column.Item().Text($"Period: {payroll.PeriodFrom:yyyy-MM-dd} to {payroll.PeriodTo:yyyy-MM-dd}");
                        column.Item().Text($"Status: {payroll.Status}");
    
                        column.Item().LineHorizontal(1);
    
                        column.Item().Text("Summary")
                            .FontSize(16)
                            .Bold();
    
                        column.Item().Text($"Total Hours: {payroll.TotalHours}");
                        column.Item().Text($"Total Amount: {payroll.TotalAmount:C}");
    
                        column.Item().LineHorizontal(1);
    
                        column.Item().Text("Breakdown")
                            .FontSize(16)
                            .Bold();
    
                        foreach (var component in payroll.Components)
                        {
                            column.Item().Text($"{component.Type} - {component.Amount:C}");
                        }
                    });
    
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Generated on ");
                            x.Span(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm"));
                        });
                });
            });
    
            return document.GeneratePdf();
        }
    }
}