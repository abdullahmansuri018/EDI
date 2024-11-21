using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiToJson.EdiSegment
{
    public class N9
    {
        public string ReferenceIdentificationQualifier { get; set; }
        public string ReferenceIdentification { get; set; }
        public string FreeFormDescription { get; set; }
        [JsonIgnore]
        public DateOnly? Date { get; set; }
        [JsonIgnore]
        public TimeOnly? Time { get; set; }
        public string TimeCode { get; set; }
        public string ReferenceIdentificationQualifier1 { get; set; }
        public string ReferenceIdentification1 { get; set; }
        public string FreeFormIdentificationQualifier1 { get; set; }
        public string ReferenceIdentification2 { get; set; }
        public string FreeFormIdentificationQualifier2 { get; set; }
        public string ReferenceIdentification3 { get; set; }

    }
}
