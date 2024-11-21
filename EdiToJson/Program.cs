using EdiToJson.EdiParser;
using EdiToJson.EdiSegment;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Edi315Parser edi315Parser = new Edi315Parser();

        string sourceFolder = @"C:\Users\AbdullahMansuri\Downloads\EdiFiles"; // Source folder
        string destinationFolder = @"C:\Users\AbdullahMansuri\Downloads\ProcessedEdiFiles"; // Destination folder

        if (!Directory.Exists(destinationFolder))
        {
            Directory.CreateDirectory(destinationFolder); // Ensure destination folder exists
        }

        try
        {
            // Get all the files in the source folder
            var files = Directory.GetFiles(sourceFolder, "*.txt");

            foreach (var filePath in files)
            {
                // Check if the file has already been processed
                string fileName = Path.GetFileName(filePath);
                string processedFileMarker = Path.Combine(destinationFolder, fileName);

                if (File.Exists(processedFileMarker))
                {
                    Console.WriteLine($"File '{fileName}' has already been processed.");
                    continue; // Skip the file if it has been processed
                }

                // Read the EDI file content
                string ediData = File.ReadAllText(filePath);

                // Parse the EDI data
                List<RequiredJson> edi315List = edi315Parser.ParseEdi315(ediData);

                // Serialize the parsed data to JSON
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
                };

                Console.WriteLine("Count: " + edi315List.Count);

                await CosmosDbHelper.CreateDatabaseAndContainer();

                foreach (var edi in edi315List)
                {
                    var partitionKey = edi.ContainerId;
                    await CosmosDbHelper.UploadToCosmosDb(edi, partitionKey);
                }

                Console.WriteLine("EDI Data Parsed and Uploaded Successfully!");

                // Copy the processed file to the destination folder to mark it as processed
                File.Copy(filePath, processedFileMarker);
                Console.WriteLine($"File '{fileName}' copied to '{destinationFolder}' as processed.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
