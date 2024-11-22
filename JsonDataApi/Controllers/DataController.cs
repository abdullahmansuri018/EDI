using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonDataApi.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JsonDataApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;
        private readonly AppDbContext _dbContext;

        public DataController(IConfiguration configuration, AppDbContext dbContext)
        {
            var cosmosDbConfig = configuration.GetSection("CosmosDb");
            _cosmosClient = new CosmosClient(cosmosDbConfig["EndpointUri"], cosmosDbConfig["PrimaryKey"]);
            _container = _cosmosClient.GetContainer(cosmosDbConfig["DatabaseName"], cosmosDbConfig["ContainerName"]);
            _dbContext = dbContext;
        }

        [Authorize]
        [HttpGet("fetch-by-containerId/{containerId}")]
        public async Task<IActionResult> FetchDataByContainerId(string containerId)
        {
            // Extract UserId and Email from JWT Claims
            var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
            {
                return Unauthorized("User information is missing from the claims.");
            }

            // Query Cosmos DB to check if there is any data for the given ContainerId
            var query = _container.GetItemQueryIterator<dynamic>(
                $"SELECT * FROM c WHERE c.ContainerId = '{containerId}'");

            var results = new List<CosmosContainer>();
            bool dataFound = false;

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                foreach (var item in response)
                {
                    var cosmosItem = JsonConvert.DeserializeObject<CosmosContainer>(item.ToString());
                    results.Add(cosmosItem);
                }
            }

            // If no data found in Cosmos DB, return 404
            if (results.Count == 0)
            {
                return NotFound($"No data found for ContainerId: {containerId}");
            }

            // If data exists in Cosmos DB, store the UserData in SQL Server
            var userContainerData = new UserContainerData
            {
                UserId = userId,
                Email = email,
                ContainerId = containerId
            };

            // Save the UserData to the SQL Server database
            _dbContext.UserContainerData.Add(userContainerData);
            await _dbContext.SaveChangesAsync();

            // Return the Cosmos DB results
            return Ok(results);
        }
    }
}
