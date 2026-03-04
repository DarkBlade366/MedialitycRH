using System.Globalization;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Payroll.DTOs;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Infrastructure.Reports
{
    public class PayrollExcelGenerator : IPayrollExcelGenerator
    {
        public byte[] Generate(
            PayrollResponse payroll,
            string employeeName,
            decimal vacationAvailableDays,
            decimal aguinaldoAccruedAmount,
            string language)
        {
            var culture = language == "en" ? new CultureInfo("en-US") : new CultureInfo("es-ES");
            var t = PayrollPdfTranslations.Get; // puedes reutilizar las traducciones

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Payroll");

            int row = 1;

            // Header
            sheet.Cells[row, 1].Value = t("Title", language);
            sheet.Cells[row, 1, row, 4].Merge = true;
            sheet.Cells[row, 1].Style.Font.Size = 18;
            sheet.Cells[row, 1].Style.Font.Bold = true;
            sheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            row += 2;

            // Employee info
            sheet.Cells[row, 1].Value = t("EmployeeInfo", language);
            sheet.Cells[row, 1].Style.Font.Bold = true;
            row++;

            sheet.Cells[row, 1].Value = $"{t("Name", language)}: {employeeName}";
            row++;
            sheet.Cells[row, 1].Value = $"{t("Period", language)}: {payroll.PeriodStart.ToString("d", culture)} - {payroll.PeriodEnd.ToString("d", culture)}";
            row++;
            sheet.Cells[row, 1].Value = $"{t("Status", language)}: {payroll.Status}";
            row += 2;

            // Components Table Header
            sheet.Cells[row, 1].Value = t("Description", language);
            sheet.Cells[row, 2].Value = t("Category", language);
            sheet.Cells[row, 3].Value = t("Amount", language);
            sheet.Cells[row, 1, row, 3].Style.Font.Bold = true;
            row++;

            // Components
            foreach (var c in payroll.Components ?? [])
            {
                sheet.Cells[row, 1].Value = c.Description;
                sheet.Cells[row, 2].Value = c.Category;
                sheet.Cells[row, 3].Value = c.Amount;
                sheet.Cells[row, 3].Style.Numberformat.Format = "#,##0.00";
                row++;
            }

            row++;

            // Totals
            sheet.Cells[row, 1].Value = t("Gross", language);
            sheet.Cells[row, 2].Value = payroll.GrossAmount;
            sheet.Cells[row, 2].Style.Numberformat.Format = "#,##0.00";
            row++;

            sheet.Cells[row, 1].Value = t("Deductions", language);
            sheet.Cells[row, 2].Value = payroll.TotalDeductions;
            sheet.Cells[row, 2].Style.Numberformat.Format = "#,##0.00";
            row++;

            sheet.Cells[row, 1].Value = t("Net", language);
            sheet.Cells[row, 2].Value = payroll.NetAmount;
            sheet.Cells[row, 2].Style.Font.Bold = true;
            sheet.Cells[row, 2].Style.Numberformat.Format = "#,##0.00";
            row += 2;

            // Additional info
            sheet.Cells[row, 1].Value = t("Vacation", language);
            sheet.Cells[row, 2].Value = vacationAvailableDays;
            row++;
            sheet.Cells[row, 1].Value = t("Aguinaldo", language);
            sheet.Cells[row, 2].Value = aguinaldoAccruedAmount;
            sheet.Cells[row, 2].Style.Numberformat.Format = "#,##0.00";

            return package.GetAsByteArray();
        }
    }
}