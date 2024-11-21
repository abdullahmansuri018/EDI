using EdiToJson.EdiSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiToJson.EdiParser
{
    public class Q2_parser
    {
        public Q2 ParseQ2(string[] segments)
        {
            Q2 q2 = new Q2();

            if (segments.Length > 1 && !string.IsNullOrWhiteSpace(segments[1]))
            {
                q2.VesselCode = segments[1];
            }

            if (segments.Length > 2 && !string.IsNullOrWhiteSpace(segments[2]))
            {
                q2.CountryCode = segments[2];
            }

            if (segments.Length > 3 && !string.IsNullOrWhiteSpace(segments[3]))
            {
                q2.Date = segments[3];
            }

            if (segments.Length > 4 && !string.IsNullOrWhiteSpace(segments[4]))
            {
                q2.Date1 = segments[4];
            }

            if (segments.Length > 5 && !string.IsNullOrWhiteSpace(segments[5]))
            {
                q2.Date2 = segments[5]; 
            }

            if (segments.Length > 6 && !string.IsNullOrWhiteSpace(segments[6]))
            {
                q2.LadingQuantity = segments[6];
            }

            if (segments.Length > 7 && !string.IsNullOrWhiteSpace(segments[7]))
            {
                q2.Weight = segments[7];
            }

            if (segments.Length > 8 && !string.IsNullOrWhiteSpace(segments[8]))
            {
                q2.WeightQualifier = segments[8];
            }

            if (segments.Length > 9 && !string.IsNullOrWhiteSpace(segments[9]))
            {
                q2.FlightNumber = segments[9];
            }

            if (segments.Length > 10 && !string.IsNullOrWhiteSpace(segments[10]))
            {
                q2.ReferenceIdentificationQualifier = segments[10];
            }

            if (segments.Length > 11 && !string.IsNullOrWhiteSpace(segments[11]))
            {
                q2.ReferenceIdentification = segments[11];
            }

            if (segments.Length > 12 && !string.IsNullOrWhiteSpace(segments[12]))
            {
                q2.VesselCodeQualifier = segments[12];
            }

            if (segments.Length > 13 && !string.IsNullOrWhiteSpace(segments[13]))
            {
                q2.VesselName = segments[13];
            }

            if (segments.Length > 14 && !string.IsNullOrWhiteSpace(segments[14]))
            {
                q2.Volume = segments[14];
            }

            if (segments.Length > 15 && !string.IsNullOrWhiteSpace(segments[15]))
            {
                q2.VolumeUnitQualifier = segments[15];
            }

            if (segments.Length > 16 && !string.IsNullOrWhiteSpace(segments[16]))
            {
                q2.WeightUnitCode = segments[16];
            }

           
            return q2;
        }
    }
}
