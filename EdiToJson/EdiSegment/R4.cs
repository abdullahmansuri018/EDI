using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiToJson.EdiSegment
{
    public class R4
    {
        public string PortFunctionCode { get; set; }
        public string LocationQualifier { get; set; }
        public string LocationIdentifier { get; set; }
        public string PortName { get; set; }
        public string CountryCode { get; set; }
        public string TermianlCode { get; set; }
        public string PierNumber { get; set; }
        public string Status { get; set; }

    }
}
