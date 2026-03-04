using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Reports
{
    public static class PayrollPdfTranslations
    {
        private static readonly Dictionary<string, Dictionary<string, string>> _data =
            new()
            {
                ["es"] = new()
                {
                    ["Title"] = "RECIBO DE NÓMINA",
                    ["EmployeeInfo"] = "Información del Empleado",
                    ["Name"] = "Nombre",
                    ["Period"] = "Periodo",
                    ["Status"] = "Estado",
                    ["Breakdown"] = "Desglose de Nómina",
                    ["Description"] = "Descripción",
                    ["Category"] = "Categoría",
                    ["Amount"] = "Monto",
                    ["Summary"] = "Resumen",
                    ["Gross"] = "Monto Bruto",
                    ["Deductions"] = "Total Deducciones",
                    ["Net"] = "Monto Neto",
                    ["Additional"] = "Información Adicional",
                    ["Vacation"] = "Días de Vacaciones Disponibles",
                    ["Aguinaldo"] = "Aguinaldo Acumulado",
                    ["GeneratedOn"] = "Generado el"
                },
                ["en"] = new()
                {
                    ["Title"] = "PAYROLL RECEIPT",
                    ["EmployeeInfo"] = "Employee Information",
                    ["Name"] = "Name",
                    ["Period"] = "Period",
                    ["Status"] = "Status",
                    ["Breakdown"] = "Payroll Breakdown",
                    ["Description"] = "Description",
                    ["Category"] = "Category",
                    ["Amount"] = "Amount",
                    ["Summary"] = "Summary",
                    ["Gross"] = "Gross Amount",
                    ["Deductions"] = "Total Deductions",
                    ["Net"] = "Net Amount",
                    ["Additional"] = "Additional Information",
                    ["Vacation"] = "Available Vacation Days",
                    ["Aguinaldo"] = "Aguinaldo Accrued",
                    ["GeneratedOn"] = "Generated on"
                }
            };

        public static string Get(string key, string lang)
        {
            if (!_data.ContainsKey(lang))
                lang = "es";

            return _data[lang][key];
        }
    }
}