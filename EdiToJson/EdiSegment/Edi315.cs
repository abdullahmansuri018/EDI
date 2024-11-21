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
        public SegmentBlock SegmentBlocks { get; set; }
        public GE GE { get; set; }
        public IEA IEA { get; set; }

        //public Edi315()
        //{
        //    SegmentBlocks = new List<SegmentBlock>();
        //}
    }
}
