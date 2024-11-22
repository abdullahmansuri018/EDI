using System;

namespace JsonDataApi.Models
{
    public class CosmosContainer
    {
        // Primary fields - excluding Cosmos DB metadata
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
        public string Vogage { get; set; }
        public string SizeType { get; set; }
        public int Fees { get; set; }
        public DateTime Date { get; set; }
    }
}