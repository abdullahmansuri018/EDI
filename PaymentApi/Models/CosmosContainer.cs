using Newtonsoft.Json;
using System;

namespace PaymentApi.Models
{
    public class CosmosContainer
    {
        [JsonProperty("id")]
        public string Id { get; set; }  
        public string ContainerId { get; set; }  
        public string TradeType { get; set; }
        public string Status { get; set; }
        public bool Holds { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Line { get; set; }  
        public string VesselName { get; set; }
        public string VesselCode { get; set; }
        public string Voyage { get; set; }
        public string SizeType { get; set; }
        public int Fees { get; set; }
    }
}