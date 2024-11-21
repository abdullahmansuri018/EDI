using EdiToJson.EdiSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiToJson.EdiParser
{
    public class N9_parser
    {
        public N9 ParseN9(string[] segments)
        {
            N9 n9 = new N9();

            if (segments.Length > 1 && !string.IsNullOrWhiteSpace(segments[1]))
            {
                n9.ReferenceIdentificationQualifier = segments[1];
            }

            if (segments.Length > 2 && !string.IsNullOrWhiteSpace(segments[2]))
            {
                n9.ReferenceIdentification = segments[2];
            }

            if (segments.Length > 3 && !string.IsNullOrWhiteSpace(segments[3]))
            {
                n9.FreeFormDescription = segments[3];
            }

            if (segments.Length > 4 && !string.IsNullOrWhiteSpace(segments[4]))
            {
                n9.Date = DateOnly.ParseExact(segments[4], "yyyyMMdd");
            }

            if (segments.Length > 5 && !string.IsNullOrWhiteSpace(segments[5]))
            {
                n9.Time = TimeOnly.ParseExact(segments[5], "HHmm");
            }

            if (segments.Length > 6 && !string.IsNullOrWhiteSpace(segments[6]))
            {
                n9.TimeCode = segments[6];
            }

            if (segments.Length > 7 && !string.IsNullOrWhiteSpace(segments[7]))
            {
                n9.ReferenceIdentificationQualifier1 = segments[7];
            }

            if (segments.Length > 8 && !string.IsNullOrWhiteSpace(segments[8]))
            {
                n9.ReferenceIdentification1 = segments[8];
            }

            if (segments.Length > 9 && !string.IsNullOrWhiteSpace(segments[9]))
            {
                n9.FreeFormIdentificationQualifier1 = segments[9];
            }

            if (segments.Length > 10 && !string.IsNullOrWhiteSpace(segments[10]))
            {
                n9.ReferenceIdentification2 = segments[10];
            }

            if (segments.Length > 11 && !string.IsNullOrWhiteSpace(segments[11]))
            {
                n9.FreeFormIdentificationQualifier2 = segments[11];
            }

            if (segments.Length > 12 && !string.IsNullOrWhiteSpace(segments[12]))
            {
                n9.ReferenceIdentification3 = segments[12];
            }

            return n9;
        }
    }
}
