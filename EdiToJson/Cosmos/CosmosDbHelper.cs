using EdiToJson.EdiSegment;
using Microsoft.Azure.Cosmos;
using System;
using System.Threading.Tasks;

public static class CosmosDbHelper
{
    private static CosmosClient cosmosClient;
    private static Container container;

    public static string cosmosEndpoint = "https://localhost:8081/";
    public static string cosmosKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
    public static string databaseId = "EDIParserDb";
    public static string containerId = "EDIContainer";

    static CosmosDbHelper()
    {
        cosmosClient = new CosmosClient(cosmosEndpoint, cosmosKey);
    }

    // Create database and container if not exists
    public static async Task CreateDatabaseAndContainer()
    {
        try
        {
            // Create or get database
            Database database = cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId).Result;
            Console.WriteLine($"Database '{databaseId}' created or already exists.");

            // Create or get container with dynamic PartitionKeyPath
            var containerProperties = new ContainerProperties
            {
                Id = containerId,
                PartitionKeyPath = "/ContainerId"  // Using 'ContainerId' as partition key
            };

            container = database.CreateContainerIfNotExistsAsync(containerProperties).Result;
            Console.WriteLine($"Container '{containerId}' created or already exists.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating database or container: {ex.Message}");
            throw;
        }
    }

    // Upload data to Cosmos DB with the dynamic PartitionKey based on the ContainerId
    public static async Task UploadToCosmosDb(RequiredJson jsonOutput, string containerId)
    {
        try
        {
            
            // Create an anonymous object to hold the data with the 'id' field and dynamic PartitionKey based on the ContainerId
            var containerItem = new
            {
                id = Guid.NewGuid().ToString(),
                ContainerId = containerId, // Directly include the ContainerId
                TradeType = jsonOutput.TradeType,
                Status = jsonOutput.Status,
                Holds = jsonOutput.Holds,
                Origin = jsonOutput.Origin,
                Destination = jsonOutput.Destination,
                line = jsonOutput.line,
                VesselName = jsonOutput.VesselName,
                VesselCode = jsonOutput.VesselCode,
                Vogage = jsonOutput.Vogage,
                SizeType = jsonOutput.SizeType,
                Fees = jsonOutput.Fees,
                Date = jsonOutput.Date// The data you're uploading
            };

            // Upload the item to Cosmos DB using the generated 'id' and 'ContainerId' as partition key
            await container.CreateItemAsync(containerItem, new PartitionKey(containerId));
            //Console.WriteLine($"Uploaded data to Cosmos DB with partition key (ContainerId): {containerId} and id: {uniqueId}.");
        }
        catch (CosmosException cosmosEx)
        {
            Console.WriteLine($"Cosmos DB error: {cosmosEx.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during Cosmos DB upload: {ex.Message}");
        }
    }
}
