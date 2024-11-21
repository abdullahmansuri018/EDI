﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiToJson.EdiSegment
{
    public class RequiredJson
    {
        public string ContainerId { get; set; }
        public string TradeType { get; set; }
        public string Status { get; set; }
        public bool Holds { get; set; }=false;
        public String Origin { get; set; }
        public String Destination { get; set; }
        public string line { get; set; }
        public string VesselName { get; set; }
        public string VesselCode { get; set; }
        public string Vogage { get; set; }
        public string SizeType { get; set; }
        public int Fees { get; set; }

        public DateOnly Date { get; set; }
    }
}