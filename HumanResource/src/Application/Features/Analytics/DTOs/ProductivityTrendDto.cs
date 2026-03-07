using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Analytics.DTOs
{
    public class ProductivityTrendDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Metric { get; set; }
    }
}