using EdiToJson.EdiSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiToJson.EdiParser
{
    public class GE_parser
    {
        public GE ParseGE(string[] segments)
        {
            GE ge = new GE();
            if (segments.Length > 1 && !string.IsNullOrWhiteSpace(segments[1]))
            {
                ge.NumberOfTransactionSetIncluded = int.Parse(segments[1]);
            }
            if (segments.Length > 2 && !string.IsNullOrWhiteSpace(segments[2]))
            {
                ge.GroupControlNumber = int.Parse(segments[2]);
            }
            return ge;
        }
    }
}
