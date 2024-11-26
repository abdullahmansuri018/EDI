using Newtonsoft.Json;
using System;

namespace PaymentApi.Models
{
    public class CosmosContainer
    {
        // Primary fields - excluding Cosmos DB metadata
        [JsonProperty("id")]
        public string Id { get; set; }  // This should match 'id' in the Cosmos DB document
        public string ContainerId { get; set; }  // This should match 'ContainerId' in the Cosmos DB document
        public string TradeType { get; set; }
        public string Status { get; set; }
        public bool Holds { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Line { get; set; }  // Use 'line' (lowercase) to match Cosmos DB document
        public string VesselName { get; set; }
        public string VesselCode { get; set; }
        public string Voyage { get; set; }
        public string SizeType { get; set; }
        public int Fees { get; set; }
        public DateTime Date { get; set; } // Ensure the format matches or convert accordingly
    }
}