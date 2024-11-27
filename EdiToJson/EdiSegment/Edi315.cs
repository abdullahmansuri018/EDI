using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiToJson.EdiSegment
{
    public class Edi315
    {
        public ISA ISA { get; set; }
        public GS GS { get; set; }
        public ST ST { get; set; }
        public B4 B4 { get; set; } 
        public N9 N9 { get; set; }
        public Q2 Q2 { get; set; }
        public SG SG { get; set; }
        public R4 R4 { get; set; }
        public SE SE { get; set; }
        public GE GE { get; set; }
        public IEA IEA { get; set; }
    }
}
