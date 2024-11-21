using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using JsonDataApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace JsonDataApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public DataController(IConfiguration configuration)
        {
            var cosmosDbConfig = configuration.GetSection("CosmosDb");
            _cosmosClient = new CosmosClient(cosmosDbConfig["EndpointUri"], cosmosDbConfig["PrimaryKey"]);
            _container = _cosmosClient.GetContainer(cosmosDbConfig["DatabaseName"], cosmosDbConfig["ContainerName"]);
        }

        // The model class CosmosItem should be defined somewhere in your code
        // Example: public class CosmosItem { ... }

        [HttpGet("fetch-all-data")]
        public async Task<IActionResult> FetchAllDataFromCosmos()


        {
            // Query to fetch all records from Cosmos DB
            var query = _container.GetItemQueryIterator<dynamic>("SELECT * FROM c");

            var results = new List<CosmosContainer>();

            // Execute the query and process results
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                foreach (var item in response)
                {
                    // Deserialize each item to CosmosItem model
                    var cosmosItem = JsonConvert.DeserializeObject<CosmosContainer>(item.ToString());
                    results.Add(cosmosItem);
                }
            }

            // If no results found, return 404
            if (results.Count == 0)
            {
                return NotFound("No data found.");
            }

            // Return the data as JSON
            return Ok(results);
        }
        [Authorize]
        [HttpGet("fetch-by-containerId/{containerId}")]
        public async Task<IActionResult> FetchDataByContainerId(string containerId)
        {
            var query = _container.GetItemQueryIterator<dynamic>(
                $"SELECT * FROM c WHERE c.ContainerId = '{containerId}'");

            var results = new List<CosmosContainer>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                foreach (var item in response)
                {
                    var cosmosItem = JsonConvert.DeserializeObject<CosmosContainer>(item.ToString());
                    results.Add(cosmosItem);
                }
            }

            if (results.Count == 0)
            {
                return NotFound($"No data found for containerId: {containerId}");
            }

            return Ok(results);
        }
    }
}
