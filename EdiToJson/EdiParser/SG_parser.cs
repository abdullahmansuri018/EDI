using EdiToJson.EdiSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiToJson.EdiParser
{
    public class SG_parser
    {
        public SG ParseSG(string[] segments)
        {
            SG sg = new SG();

            if (segments.Length > 1 && !string.IsNullOrWhiteSpace(segments[1]))
            {
                sg.ShpimentStatusCode = segments[1];
            }

            if (segments.Length > 2 && !string.IsNullOrWhiteSpace(segments[2]))
            {
                sg.StatusReasonCode = segments[2];
            }

            if (segments.Length > 3 && !string.IsNullOrWhiteSpace(segments[3]))
            {
                sg.DispositionCode = segments[3];
            }

            if (segments.Length > 4 && !string.IsNullOrWhiteSpace(segments[4]))
            {  
                sg.Date = DateOnly.ParseExact(segments[4], "yyyyMMdd"); 
            }

            if (segments.Length > 5 && !string.IsNullOrWhiteSpace(segments[5]))
            {
                sg.Time = TimeOnly.ParseExact(segments[5], "HHmm");
            }

            if (segments.Length > 6 && !string.IsNullOrWhiteSpace(segments[6]))
            {
                sg.TimeCode = segments[6];
            }

            return sg;
        }
    }
}
