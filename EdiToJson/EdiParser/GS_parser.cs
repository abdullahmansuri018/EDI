using EdiToJson.EdiSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiToJson.EdiParser
{
    public class GS_parser
    {
        public GS ParseGS(string[] segments)
        { 
            GS gs = new GS();
            if (segments.Length > 1 && !string.IsNullOrWhiteSpace(segments[1]))
            {
                gs.FunctionalIdentifierCode = segments[1];
            }
            if (segments.Length > 2 && !string.IsNullOrWhiteSpace(segments[2]))
            {
                gs.FunctionalSendersCode = segments[2];
            }
            if (segments.Length > 3 && !string.IsNullOrWhiteSpace(segments[3]))
            {
                gs.FunctionalRecieversCode = segments[3];
            }
            if (segments.Length > 4 && !string.IsNullOrWhiteSpace(segments[4]))
            {
                gs.GroupDate = DateOnly.ParseExact(segments[4], "yyyyMMdd");
            }
            if (segments.Length > 5 && !string.IsNullOrWhiteSpace(segments[5]))
            {
                gs.GroupTime = TimeOnly.ParseExact(segments[5], "HHmm");
            }
            if (segments.Length > 6 && !string.IsNullOrWhiteSpace(segments[6]))
            {
                gs.GroupControlNumber = segments[6];
            }
            if (segments.Length > 7 && !string.IsNullOrWhiteSpace(segments[7]))
            {
                gs.ResponsibleAgencyCode = segments[7];
            }
            if (segments.Length > 8 && !string.IsNullOrWhiteSpace(segments[8]))
            {
                gs.Version = segments[8];
            }
            return gs;
        }
    }
}
