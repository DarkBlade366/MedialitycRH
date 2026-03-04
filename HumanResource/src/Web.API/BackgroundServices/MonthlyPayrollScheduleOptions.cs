using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.API.BackgroundServices
{
    public class MonthlyPayrollScheduleOptions
    {
        public bool Enabled { get; set; } = true;
        public int RunDayOfMonth { get; set; } = 1;
        public int RunHourUtc { get; set; } = 0;
        public int RunMinuteUtc { get; set; } = 5;
    }
}
