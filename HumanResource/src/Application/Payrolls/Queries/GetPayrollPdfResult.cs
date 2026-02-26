using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Payrolls.Queries
{
    public class GetPayrollPdfResult
    {
        public byte[]? FileBytes { get; set; }
        public string FileName { get; set; } = string.Empty;
    }
}