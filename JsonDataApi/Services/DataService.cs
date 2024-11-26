using JsonDataApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JsonDataApi.Services
{
    public class DataService : IDataService
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;
        private readonly AppDbContext _dbContext;

        public DataService(IConfiguration configuration, AppDbContext dbContext)
        {
            var cosmosDbConfig = configuration.GetSection("CosmosDb");
            _cosmosClient = new CosmosClient(cosmosDbConfig["EndpointUri"], cosmosDbConfig["PrimaryKey"]);
            _container = _cosmosClient.GetContainer(cosmosDbConfig["DatabaseName"], cosmosDbConfig["ContainerName"]);
            _dbContext = dbContext;
        }

        public async Task<List<CosmosContainer>> FetchDataByContainerId(string containerId, string userId, string email)
        {
            // Check if there is already a record in UserContainerData for this userId and containerId
            var existingData = await _dbContext.UserContainerData.FirstOrDefaultAsync(uc =>  uc.ContainerId == containerId);

            if (existingData != null)
            {
                // If the record exists, return null or throw an error as you prefer
                throw new InvalidOperationException("Container is not available to add");
            }

            // Query Cosmos DB to check if there is any data for the given ContainerId
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

            // If no data found in Cosmos DB, return null
            if (results.Count == 0)
            {
                return null;
            }

            // Save the User's data to SQL Server for later reference
            var userContainerData = new UserContainerData
            {
                UserId = userId,
                Email = email,
                ContainerId = containerId
            };

            _dbContext.UserContainerData.Add(userContainerData);
            await _dbContext.SaveChangesAsync();

            return results;
        }

        public async Task<List<CosmosContainer>> FetchDataByUserId(string userId)
        {
            // Query SQL Server (MSSQL) to get all ContainerIds associated with the user
            var userContainerData = await _dbContext.UserContainerData
                .Where(x => x.UserId == userId)
                .ToListAsync();

            if (userContainerData.Count == 0)
            {
                return new List<CosmosContainer>();  // Return empty array if no data found
            }

            var fullContainerData = new List<CosmosContainer>();

            foreach (var userData in userContainerData)
            {
                var containerId = userData.ContainerId;
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

                fullContainerData.AddRange(results);
            }

            return fullContainerData;
        }

        public async Task<bool> RemoveContainer(string containerId, string userId)
        {
            var containerData = await _dbContext.UserContainerData
                .Where(x => x.ContainerId == containerId && x.UserId == userId)
                .FirstOrDefaultAsync();

            if (containerData == null)
            {
                return false;
            }

            _dbContext.UserContainerData.Remove(containerData);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
