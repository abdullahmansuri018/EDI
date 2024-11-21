using EdiToJson.EdiSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiToJson.EdiParser
{
    public class SE_parser
    {
        public SE ParseSE(string[] segments)
        {
            SE se = new SE();
            if (segments.Length > 1 && !string.IsNullOrWhiteSpace(segments[1]))
            {
                se.NumberOfIncludedSegment = int.Parse(segments[1]);
            }

            if (segments.Length > 2 && !string.IsNullOrWhiteSpace(segments[2]))
            {
                se.TransactionSetControlNumber = int.Parse(segments[2]);
            }

            return se;
        }
    }
}
