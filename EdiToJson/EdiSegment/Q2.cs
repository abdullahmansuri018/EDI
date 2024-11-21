using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EdiToJson.EdiSegment
{
    public class Q2
    {
        public string VesselCode { get; set; }
        public string CountryCode { get; set; }
        public string Date { get; set; }
        public string Date1 { get; set; }
        public string Date2 { get; set; }
        public string LadingQuantity { get; set; }
        public string Weight { get; set; }
        public string WeightQualifier { get; set; }
        public string FlightNumber { get; set; }
        public string ReferenceIdentificationQualifier { get; set; }
        public string ReferenceIdentification { get; set; }
        public string VesselCodeQualifier { get; set; }
        public string VesselName { get; set; }
        public string Volume { get; set; }
        public string VolumeUnitQualifier { get; set; }
        public string WeightUnitCode { get; set; }

    }
}
