using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiToJson.EdiSegment
{
    public class GS
    {
        public string FunctionalIdentifierCode { get; set; }
        public string FunctionalSendersCode { get; set; }
        public string FunctionalRecieversCode { get; set; }
        public DateOnly GroupDate { get; set; }
        public TimeOnly GroupTime { get; set; }
        public string GroupControlNumber { get; set; }

        public string ResponsibleAgencyCode { get; set; }
        public string Version { get; set; }
    }
}
