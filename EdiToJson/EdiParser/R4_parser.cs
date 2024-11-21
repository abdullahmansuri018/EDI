using EdiToJson.EdiSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiToJson.EdiParser
{
    public class R4_parser
    {
        public R4 ParseR4(string[] segments)
        {
            R4 r4 = new R4();

            if (segments.Length > 1 && !string.IsNullOrWhiteSpace(segments[1]))
            {
                r4.PortFunctionCode = segments[1];
            }

            if (segments.Length > 2 && !string.IsNullOrWhiteSpace(segments[2]))
            {
                r4.LocationQualifier = segments[2];
            }

            if (segments.Length > 3 && !string.IsNullOrWhiteSpace(segments[3]))
            {
                r4.LocationIdentifier = segments[3];
            }

            if (segments.Length > 4 && !string.IsNullOrWhiteSpace(segments[4]))
            {
                r4.PortName = segments[4];
            }

            if (segments.Length > 5 && !string.IsNullOrWhiteSpace(segments[5]))
            {
                r4.CountryCode = segments[5];
            }

            if (segments.Length > 6 && !string.IsNullOrWhiteSpace(segments[6]))
            {
                r4.TermianlCode = segments[6];
            }

            if (segments.Length > 7 && !string.IsNullOrWhiteSpace(segments[7]))
            {
                r4.PierNumber = segments[7];
            }

            if (segments.Length > 8 && !string.IsNullOrWhiteSpace(segments[8]))
            {
                r4.Status = segments[8];
            }

            return r4;
        }
    }
}
