using EdiToJson.EdiSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiToJson.EdiParser
{
    public class B4_parser
    {
        public B4 ParseB4(string[] segments)
        {
            B4 b4 = new B4();
            if (segments.Length > 1 && !string.IsNullOrWhiteSpace(segments[1]))
            {
                b4.SpecialHandlingCode = segments[1];
            }
            if (segments.Length > 2 && !string.IsNullOrWhiteSpace(segments[2]))
            {
                b4.InquiryRequestCode = segments[2];
            }
            if (segments.Length > 3 && !string.IsNullOrWhiteSpace(segments[3]))
            {
                b4.ShipmentStatusCode = segments[3];
            }
            if (segments.Length > 4 && !string.IsNullOrWhiteSpace(segments[4]))
            {
                b4.ReleaseDate = DateOnly.ParseExact(segments[4], "yyyyMMdd");
            }
            if (segments.Length > 5 && !string.IsNullOrWhiteSpace(segments[5]))
            {
                b4.ReleaseTime = TimeOnly.ParseExact(segments[5], "HHmm");
            }
            if (segments.Length > 6 && !string.IsNullOrWhiteSpace(segments[6]))
            {
                b4.StatusLocation = segments[6];
            }
            if (segments.Length > 7 && !string.IsNullOrWhiteSpace(segments[7]))
            {
                b4.EquipmentIntial = segments[7];
            }
            if (segments.Length > 8 && !string.IsNullOrWhiteSpace(segments[8]))
            {
                b4.EquipmentNumber = segments[8];
            }
            if (segments.Length > 9 && !string.IsNullOrWhiteSpace(segments[9]))
            {
                b4.EquipmentStatusCode = segments[9];
            }
            if (segments.Length > 10 && !string.IsNullOrWhiteSpace(segments[10]))
            {
                b4.EquipmentType = segments[10];
            }
            if (segments.Length > 11 && !string.IsNullOrWhiteSpace(segments[11]))
            {
                b4.LocationIdentifier = segments[11];
            }
            if (segments.Length > 12 && !string.IsNullOrWhiteSpace(segments[12]))
            {
                b4.LocationQualifier = segments[12];
            }
            if (segments.Length > 12 && !string.IsNullOrWhiteSpace(segments[12]))
            {
                b4.EquipmentNoCheckDigit = segments[12];
            }
            return b4;



        }
    }
}
