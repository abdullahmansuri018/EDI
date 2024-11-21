using EdiToJson.EdiSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EdiToJson.EdiParser;
namespace EdiToJson.EdiParser
{
    public class IEA_parser
    {

        public  IEA ParseIEA(string[] segments)
        {
                 IEA iea = new IEA();
            
                if (segments.Length > 1 && !string.IsNullOrWhiteSpace(segments[1]))
                {
                    iea.NumberOfIncludedFunctionalGroups = int.Parse(segments[1]);
                }
                if (segments.Length > 2 && !string.IsNullOrWhiteSpace(segments[2]))
                {
                    iea.InterchangeControlNumber = int.Parse(segments[2]);
                }
            
            return iea;
        }

        
        
        
    }
}
