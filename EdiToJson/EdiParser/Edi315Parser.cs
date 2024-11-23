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



            Edi315 edi315 = null;
            ISA isa = null;
            GS gs = null;
            SegmentBlock currentBlock = null;  // Current block between ST and SE

            var lines = ediData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            bool isParsingST = false;  // Flag to track whether we're parsing between ST and SE segments

            foreach (var line in lines)
            {
                var segments = line.Split('*').Select(e => e.Trim()).ToArray(); // Split the segment by '*' delimiter

                switch (segments[0])
                {
                    case "ISA":
                        isa = new ISA();
                        isa = isaParser.ParseISA(segments);  // Parse ISA only once
                        break;
                    case "GS":
                        gs = new GS();
                        gs = gsParser.ParseGS(segments);  // Parse GS only once
                        break;
                    case "ST":
                        requiredjson = new RequiredJson();
                        currentBlock = new SegmentBlock();
                        currentBlock.ST = stParser.ParseST(segments); // Parse ST
                        isParsingST = true;
                        break;
                    case "B4":
                        if (isParsingST && currentBlock != null)
                        {
                            var b4Segment = b4Parser.ParseB4(segments);  // Parse and get B4 segment
                            currentBlock.B4s.Add(b4Segment);  // Parse and add B4 segment
                            requiredjson.ContainerId = $"{b4Segment.EquipmentIntial}{b4Segment.EquipmentNumber}";
                            requiredjson.TradeType = b4Segment.SpecialHandlingCode;
                            requiredjson.Status = b4Segment.ShipmentStatusCode;
                            requiredjson.SizeType = b4Segment.EquipmentType;
                            requiredjson.Date = b4Segment.ReleaseDate;
                        }
                        break;
                    case "N9":
                        if (isParsingST && currentBlock != null)
                        {
                            var n9Segment = n9Parser.ParseN9(segments);
                            currentBlock.N9s.Add(n9Segment);  // Parse and add N9 segment
                            if (n9Segment.ReferenceIdentificationQualifier == "SCA")
                            {
                                requiredjson.line = n9Segment.ReferenceIdentification;
                            }
                            if (n9Segment.ReferenceIdentificationQualifier == "22" || n9Segment.ReferenceIdentificationQualifier == "4I" || n9Segment.ReferenceIdentificationQualifier == "4I1" ||
                            n9Segment.ReferenceIdentificationQualifier == "4I2" || n9Segment.ReferenceIdentificationQualifier == "4I3" || n9Segment.ReferenceIdentificationQualifier == "4I4" ||
                            n9Segment.ReferenceIdentificationQualifier == "4I5" || n9Segment.ReferenceIdentificationQualifier == "4I6" || n9Segment.ReferenceIdentificationQualifier == "4I7" ||
n9Segment.ReferenceIdentificationQualifier == "4I8" || n9Segment.ReferenceIdentificationQualifier == "4I9" || n9Segment.ReferenceIdentificationQualifier == "4IV" ||
n9Segment.ReferenceIdentificationQualifier == "4IE" || n9Segment.ReferenceIdentificationQualifier == "TMF" || n9Segment.ReferenceIdentificationQualifier == "CTF" ||
n9Segment.ReferenceIdentificationQualifier == "HZP" || n9Segment.ReferenceIdentificationQualifier == "FCH" || n9Segment.ReferenceIdentificationQualifier == "BBC" ||
n9Segment.ReferenceIdentificationQualifier == "DVF" || n9Segment.ReferenceIdentificationQualifier == "USD" || n9Segment.ReferenceIdentificationQualifier == "GEN" ||
n9Segment.ReferenceIdentificationQualifier == "SCR" || n9Segment.ReferenceIdentificationQualifier == "WCR" || n9Segment.ReferenceIdentificationQualifier == "GEC" ||
n9Segment.ReferenceIdentificationQualifier == "DRC" || n9Segment.ReferenceIdentificationQualifier == "ATG" || n9Segment.ReferenceIdentificationQualifier == "EGF" ||
n9Segment.ReferenceIdentificationQualifier == "IGF" || n9Segment.ReferenceIdentificationQualifier == "OOG" || n9Segment.ReferenceIdentificationQualifier == "RF" ||
n9Segment.ReferenceIdentificationQualifier == "DWT" || n9Segment.ReferenceIdentificationQualifier == "FI")
                            {
                                requiredjson.Fees+=Convert.ToInt32(n9Segment.ReferenceIdentification);
                                //decimal feeAmount = 0;

                                // Attempt to parse the fee value from the segment
                                //if (requiredjson.Fees > 0)
                                //{
                                //    requiredjson.Holds = true;
                                //}
                                //else
                                //{ 
                                //    requiredjson.Holds= false;
                                //}
                            }
                        }
                        break;
                    case "SG":
                        if (isParsingST && currentBlock != null)
                        {
                            var sgSegment = sgParser.ParseSG(segments);
                            currentBlock.SGs.Add(sgSegment);  // Parse and add SG segment
                        }
                        break;
                    case "Q2":
                        if (isParsingST && currentBlock != null)
                        {
                            var q2Segment = q2Parser.ParseQ2(segments);
                            currentBlock.Q2 = q2Segment;  // Parse and add Q2 segment
                            requiredjson.VesselName = currentBlock.Q2.VesselName;
                            requiredjson.VesselCode = currentBlock.Q2.VesselCode;
                            requiredjson.Voyage = currentBlock.Q2.FlightNumber;
                        }
                        break;
                    case "R4":
                        if (isParsingST && currentBlock != null)
                        {
                            var r4Segment = r4Parser.ParseR4(segments);
                            currentBlock.R4s.Add(r4Segment);  // Parse and add R4 segment
                            if (r4Segment.PortFunctionCode == "L")
                            {
                                requiredjson.Origin = r4Segment.PortName;
                            }
                            if (r4Segment.PortFunctionCode == "D")
                            {
                                requiredjson.Destination = r4Segment.PortName;
                            }

                        }
                        break;
                    case "SE":
                        if (isParsingST && currentBlock != null)
                        {
                            edi315 = new Edi315();
                            var seSegment = seParser.ParseSE(segments);
                            currentBlock.SE = seSegment;  // Parse SE and close the block
                            requiredJsonList.Add(requiredjson);

                            currentBlock = null;
                            edi315 = null;
                            requiredjson = null;
                        }
                        break;
                    default:
                        Console.WriteLine($"Unknown segment: {segments[0]}");
                        break;
                }
            }

            return requiredJsonList;  // Return the list of required JSON objects
        }

    }


}        



                    //case "GE":
                    //    if (!geParsed)
                    //    {
                    //        edi315.GE = geParser.ParseGE(segments);  // Parse GE only once
                    //        geParsed = true;
                    //    }
                    //    break;
                    //case "IEA":
                    //    if (!ieaParsed)
                    //    {
                    //        edi315.IEA = ieaParser.ParseIEA(segments);  // Parse IEA only once
                    //        ieaParsed = true;
                    //    }
                    //    break;
                    //default:
                    //    Console.WriteLine($"Unknown segment: {segments[0]}");
                    //    break;