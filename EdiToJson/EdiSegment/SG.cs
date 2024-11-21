using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiToJson.EdiSegment
{
    public class SG
    {
        public string ShpimentStatusCode { get; set; }
        public string StatusReasonCode { get; set; }
        public string DispositionCode { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string TimeCode { get; set; }

    }
}
