using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.Core
{
    public sealed class ReportInfo
    {
        public ReportType ReportType { get; set; }
        public string Name { get; set; }
        public string Information { get; set; }
        public ReportLevel DisplayLevel { get; set; }
        public bool ShowActive { get; set; }
        public bool ShowSectionNameFormat { get; set; }
        public bool ShowStudentNameFormat { get; set; }
        public bool ShowAddressFormat { get; set; }
    }
}
