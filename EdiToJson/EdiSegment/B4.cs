using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiToJson.EdiSegment
{
    /// <summary>
    /// 
    /// </summary>
    public class B4
    {
        /// <summary>
        /// 
        /// </summary>
        public string SpecialHandlingCode { get; set; }
        public string InquiryRequestCode { get; set; }
        public string ShipmentStatusCode { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public TimeOnly ReleaseTime { get; set; }
        public string StatusLocation { get; set; }
        public string EquipmentIntial { get; set; }
        public string EquipmentNumber { get; set; }
        public string EquipmentStatusCode { get; set; }
        public string EquipmentType { get; set; }
        public string LocationIdentifier { get; set; }
        public string LocationQualifier { get; set; }
        //
        public string EquipmentNoCheckDigit { get; set; }
    }
}
