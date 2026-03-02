using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Employees.DTOs
{
    public class VacationBalanceResponse
    {
        public Guid EmployeeId { get; set; }
        public decimal AccruedDays { get; set; }
        public decimal UsedDays { get; set; }
        public decimal AvailableDays { get; set; }
    }
}