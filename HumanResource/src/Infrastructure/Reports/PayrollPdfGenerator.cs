using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Application.Features.Payrolls.Payroll.DTOs;
using Application.Common.Interfaces;

namespace Infrastructure.Reports
{
    public class PayrollPdfGenerator : IPayrollPdfGenerator
    {
        public byte[] Generate(
        PayrollResponse payroll,
        string employeeName,
        decimal vacationAvailableDays,
        decimal aguinaldoAccruedAmount,
        string language)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var culture = language == "en"
                ? new CultureInfo("en-US")
                : new CultureInfo("es-ES");

            var t = PayrollPdfTranslations.Get;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);

                    page.Header()
                        .Text(t("Title", language))
                        .FontSize(20)
                        .Bold()
                        .AlignCenter();

                    page.Content().Column(column =>
                    {
                        column.Spacing(15);

                        column.Item().Text(t("EmployeeInfo", language))
                            .FontSize(14)
                            .SemiBold();

                        column.Item().Text($"{t("Name", language)}: {employeeName}");
                        column.Item().Text($"{t("Period", language)}: {payroll.PeriodStart.ToString("d", culture)} - {payroll.PeriodEnd.ToString("d", culture)}");
                        column.Item().Text($"{t("Status", language)}: {payroll.Status}");

                        column.Item().LineHorizontal(1);

                        column.Item().Text(t("Breakdown", language))
                            .FontSize(14)
                            .SemiBold();

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text(t("Description", language)).Bold();
                                header.Cell().AlignRight().Text(t("Category", language)).Bold();
                                header.Cell().AlignRight().Text(t("Amount", language)).Bold();
                            });

                            foreach (var component in payroll.Components ?? [])
                            {
                                table.Cell().Text(component.Description);
                                table.Cell().AlignRight().Text(component.Category);
                                table.Cell().AlignRight().Text(
                                    component.Amount.ToString("C", culture));
                            }
                        });

                        column.Item().LineHorizontal(1);

                        column.Item().Text(t("Summary", language))
                            .FontSize(14)
                            .SemiBold();

                        column.Item().Text($"{t("Gross", language)}: {payroll.GrossAmount.ToString("C", culture)}");
                        column.Item().Text($"{t("Deductions", language)}: {payroll.TotalDeductions.ToString("C", culture)}");
                        column.Item().Text($"{t("Net", language)}: {payroll.NetAmount.ToString("C", culture)}")
                            .Bold();

                        column.Item().LineHorizontal(1);

                        column.Item().Text(t("Additional", language))
                            .FontSize(14)
                            .SemiBold();

                        column.Item().Text($"{t("Vacation", language)}: {vacationAvailableDays}");
                        column.Item().Text($"{t("Aguinaldo", language)}: {aguinaldoAccruedAmount.ToString("C", culture)}");
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text($"{t("GeneratedOn", language)} {DateTime.UtcNow.ToString("g", culture)}");
                });
            });

            return document.GeneratePdf();
        }   
    }   
}   