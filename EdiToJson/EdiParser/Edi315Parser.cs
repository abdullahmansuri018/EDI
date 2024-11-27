using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdiToJson.EdiSegment;

namespace EdiToJson.EdiParser
{
    public class Edi315Parser
    {
        private readonly ISA_parser isaParser = new ISA_parser();
        private readonly GS_parser gsParser = new GS_parser();
        private readonly ST_parser stParser = new ST_parser();
        private readonly B4_parser b4Parser = new B4_parser();
        private readonly N9_parser n9Parser = new N9_parser();
        private readonly SG_parser sgParser = new SG_parser();
        private readonly R4_parser r4Parser = new R4_parser();
        private readonly SE_parser seParser = new SE_parser();
        private readonly Q2_parser q2Parser = new Q2_parser();
        private readonly GE_parser geParser = new GE_parser();
        private readonly IEA_parser ieaParser = new IEA_parser();

        public List<RequiredJson> ParseEdi315(string ediData)
        {
            List<RequiredJson> requiredJsonList = new List<RequiredJson>();
            RequiredJson requiredjson = null;

            var lines = ediData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var segments = line.Split('*').Select(e => e.Trim()).ToArray(); // Split the segment by '*' delimiter

                switch (segments[0])
                {
                    case "ISA":
                         var isaSegment = isaParser.ParseISA(segments);  
                        break;
                    case "GS":
                       var  gsSegment = gsParser.ParseGS(segments); 
                        break;
                    case "ST":
                        requiredjson = new RequiredJson();
                        var stSegment = stParser.ParseST(segments); 
                        break;
                    case "B4":
                            var b4Segment = b4Parser.ParseB4(segments);  
                            requiredjson.ContainerId = $"{b4Segment.EquipmentIntial}{b4Segment.EquipmentNumber}";
                            requiredjson.TradeType = b4Segment.SpecialHandlingCode;
                            requiredjson.Status = b4Segment.ShipmentStatusCode;
                            requiredjson.SizeType = b4Segment.EquipmentType;
                        break;
                    case "N9":
                            var n9Segment = n9Parser.ParseN9(segments);
                            if (n9Segment.ReferenceIdentificationQualifier == "SCA")
                            {
                                requiredjson.line = n9Segment.ReferenceIdentification;
                            }
                            var validQualifiers = new List<string>
                            {
                             "22", "4I", "4I1", "4I2", "4I3", "4I4", "4I5", "4I6", "4I7", "4I8",
                             "4I9", "4IV", "4IE", "TMF", "CTF", "HZP", "FCH", "BBC", "DVF", "USD",
                              "GEN", "SCR", "WCR", "GEC", "DRC", "ATG", "EGF", "IGF", "OOG", "RF",
                              "DWT", "FI"
                            };
                            if (validQualifiers.Contains(n9Segment.ReferenceIdentificationQualifier))
                            {
                                requiredjson.Fees+=Convert.ToInt32(n9Segment.ReferenceIdentification);
                                if (requiredjson.Fees > 0)
                                {
                                   requiredjson.Holds = true;
                                }
                                else
                                { 
                                   requiredjson.Holds= false;
                                }
                            }
                        
                        break;
                    case "SG":
                       
                            var sgSegment = sgParser.ParseSG(segments);
                   
                        break;
                    case "Q2":
                      
                            var q2Segment = q2Parser.ParseQ2(segments);
                            requiredjson.VesselName = q2Segment.VesselName;
                            requiredjson.VesselCode = q2Segment.VesselCode;
                            requiredjson.Voyage = q2Segment.FlightNumber;
                        
                        break;
                    case "R4":
                       
                            var r4Segment = r4Parser.ParseR4(segments);
                            if (r4Segment.PortFunctionCode == "L")
                            {
                                requiredjson.Origin = r4Segment.PortName;
                            }
                            if (r4Segment.PortFunctionCode == "D")
                            {
                                requiredjson.Destination = r4Segment.PortName;
                            }

                        
                        break;
                    case "SE":
                            var seSegment = seParser.ParseSE(segments);
                            requiredJsonList.Add(requiredjson);
                            requiredjson = null;
                        
                        break;
                    case "GE":
                            var geSegment = geParser.ParseGE(segments);
                           
                        
                        break;
                    case "IEA":
                        
                            var ieaSegment = ieaParser.ParseIEA(segments);                           
                        break;
                    default:
                        Console.WriteLine($"Unknown segment: {segments[0]}");
                        break;
                }
            }

            return requiredJsonList; 
        }

    }


}        



                    //
                    //default:
                    //    Console.WriteLine($"Unknown segment: {segments[0]}");
                    //    break;