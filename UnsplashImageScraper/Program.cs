using System.Net.Http;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;

class Program
{
    private static readonly HttpClient client = new HttpClient();
    private static readonly Random random = new Random();

    static async Task Main(string[] args)
    {
        // Set up the API endpoint URL and API access key
        string apiUrl = "https://api.unsplash.com/photos/random";
        string accessKey = "YOU API KEY HERE"; // Replace with your API key

        // Define the directory path
        string directoryPath = "images";

        // Create the directory if it doesn't exist
        if (!Directory.Exists(directoryPath))
        {
            Console.WriteLine("Creating images directory.");
            Directory.CreateDirectory(directoryPath);
            Console.WriteLine("Directory created.");
        }

        Console.WriteLine("Starting scraper.");

        for (var i = 0; i < 100; i++)
        {
            try
            {
                // Generate a random image URL asynchronously
                string imageUrl = await GetRandomImageUrlAsync(apiUrl, accessKey);

                // Download and save the image asynchronously
                await DownloadImageAsync(imageUrl, directoryPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            Console.WriteLine("Waiting 5 seconds before next request...");
            await Task.Delay(5000);
        }
    }

    static async Task<string> GetRandomImageUrlAsync(string apiUrl, string accessKey)
    {
        // Build the request URI
        string requestUri = $"{apiUrl}?client_id={accessKey}";

        // Send the GET request
        HttpResponseMessage response = await client.GetAsync(requestUri);
        response.EnsureSuccessStatusCode(); // Throws an exception if not successful

        // Read the response content as a string
        string responseJson = await response.Content.ReadAsStringAsync();

        // Deserialize the JSON using System.Text.Json
        JsonDocument jsonDoc = JsonDocument.Parse(responseJson);

        return jsonDoc.RootElement.GetProperty("urls").GetProperty("regular").GetString();
    }

    static async Task DownloadImageAsync(string imageUrl, string directoryPath)
    {
        // Get the image data as a byte array
        byte[] imageData = await client.GetByteArrayAsync(imageUrl);

        // Define the file path with a unique name
        string filePath = Path.Combine(directoryPath, $"sh_image{random.NextInt64(0, 300000000)}.jpg");

        // Save the image to disk
        await File.WriteAllBytesAsync(filePath, imageData);

        Console.WriteLine($"Image downloaded and saved to {filePath}");
    }
}