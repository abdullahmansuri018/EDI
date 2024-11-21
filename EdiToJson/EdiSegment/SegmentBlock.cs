using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiToJson.EdiSegment
{
    public class SegmentBlock
    {
        public ST ST { get; set; }
        public List<B4> B4s { get; set; } = new List<B4>();
        public List<N9> N9s { get; set; } = new List<N9>();
        public Q2 Q2 { get; set; }
        public List<SG> SGs { get; set; } = new List<SG>();
        public List<R4> R4s { get; set; } = new List<R4>();
        public SE SE { get; set; }
    }
}
