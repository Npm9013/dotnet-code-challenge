using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Models
{
    // Data model for ReportingStructure
    public class ReportingStructure : Employee
    {
        public Employee Employee { get; set; }
        public int NumberOfReports { get; set; }
    }
}
