using EdiToJson.EdiSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiToJson.EdiParser
{
    public class ST_parser
    {
        public ST ParseST(string[] segments)
        { 
            ST st = new ST();
            if (segments.Length > 1 && !string.IsNullOrWhiteSpace(segments[1]))
            {
                st.TransactionSetControlNumber = segments[1];
            }

            if (segments.Length > 2 && !string.IsNullOrWhiteSpace(segments[2]))
            {
                st.TransactionSetIdenrifierCode = segments[2];
            }
            return st;
        }
    }
}
