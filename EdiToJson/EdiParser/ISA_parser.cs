using EdiToJson.EdiSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiToJson.EdiParser
{
    public class ISA_parser
    {
        public ISA ParseISA(string[] segments)
        { 
            ISA isa= new ISA();
            if (segments.Length > 1 && !string.IsNullOrWhiteSpace(segments[1]))
            {
                isa.AuthorizationInformationQualifier = segments[1];
            }
            if (segments.Length > 2 && !string.IsNullOrWhiteSpace(segments[2]))
            {
                isa.AuthorizationInformation = segments[2];
            }
            if (segments.Length > 3 && !string.IsNullOrWhiteSpace(segments[3]))
            {
                isa.SecurityInformationQualifier = segments[3];
            }
            if (segments.Length > 4 && !string.IsNullOrWhiteSpace(segments[4]))
            {
                isa.SecurityInformation = segments[4];
            }
            if (segments.Length > 5 && !string.IsNullOrWhiteSpace(segments[5]))
            {
                isa.InterchangeIdQualifier = segments[5];
            }
            if (segments.Length > 6 && !string.IsNullOrWhiteSpace(segments[6]))
            {
                isa.InterchangeSenderId = segments[6];
            }
            if (segments.Length > 7 && !string.IsNullOrWhiteSpace(segments[7]))
            {
                isa.InterchangeIdQualifier2 = segments[7];
            }
            if (segments.Length > 8 && !string.IsNullOrWhiteSpace(segments[8]))
            {
                isa.InterchangeReceiverId = segments[8];
            }
            if (segments.Length > 9 && !string.IsNullOrWhiteSpace(segments[9]))
            {
                isa.Date = DateOnly.ParseExact(segments[9],"yyMMdd");
            }
            if (segments.Length > 10 && !string.IsNullOrWhiteSpace(segments[10]))
            {
                isa.Time = TimeOnly.ParseExact(segments[10],"HHmm");
            }
            if (segments.Length > 11 && !string.IsNullOrWhiteSpace(segments[11]))
            {
                isa.InterchangeControlStandardIdCode = segments[11];
            }
            if (segments.Length > 12 && !string.IsNullOrWhiteSpace(segments[12]))
            {
                isa.InterchangeVersion = segments[12];
            }
            if (segments.Length > 13 && !string.IsNullOrWhiteSpace(segments[13]))
            {
                isa.InterchangeControlNbr = segments[13];
            }
            if (segments.Length > 14 && !string.IsNullOrWhiteSpace(segments[14]))
            {
                isa.AcknowlegdementRequested = segments[14];
            }
            if (segments.Length > 15 && !string.IsNullOrWhiteSpace(segments[15]))
            {
                isa.TestIndicator = segments[15];
            }
            if (segments.Length > 16 && !string.IsNullOrWhiteSpace(segments[16]))
            {
                isa.SubElementSeparator = segments[16];
            }
            return isa;




        }
    }
}
