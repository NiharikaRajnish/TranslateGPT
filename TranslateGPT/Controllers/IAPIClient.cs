using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using TranslateGPT.Models;
using TranslateGPT.DTOs;


// Interface that all API clients must implement
public interface IApiClient
{
    // Method to translate text to a given language
    Task<string> Translate(string query, string language);
}

// Implementation of the IApiClient for OpenAI's API
public class OpenAIClient : IApiClient
{
    private readonly HttpClient _httpClient; // For making HTTP requests

    // Constructor to initialize HttpClient and set API key from configuration
    public OpenAIClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;

        // Add Authorization header with OpenAI API key
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuration["OpenAI:ApiKey"]}");
    }

    // Implementation of the Translate method using OpenAI's GPT API
    public async Task<string> Translate(string query, string language)
    {

        // Define the request payload for OpenAI's API
        var payload = new
        {
            model = "gpt-3", // GPT model to use
            messages = new object[]
            {
                
                new { role = "system", content = $"Translate to {language}" }, // System message
                new { role = "user", content = query } // User's input message
            },
            temperature = 0, // Ensure deterministic output
            max_tokens = 256 // Limit the response length
        };

        // Serialize the payload to JSON and prepare the HTTP request content
        var httpContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

        // Send the HTTP POST request to OpenAI's API
        var responseMessage = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", httpContent);

        // Ensure the response is successful (throws exception if not)
        responseMessage.EnsureSuccessStatusCode();

        // Read the response as a JSON string
        var responseJson = await responseMessage.Content.ReadAsStringAsync();

        // Deserialize the JSON response to extract the translated text
        var response = JsonConvert.DeserializeObject<OpenAIResponse>(responseJson);
        Console.WriteLine(response);

        // Return the translated text from the response
        return response?.Choices[0]?.Message?.Content ?? string.Empty;
    }
}

// Factory class to create instances of API clients
public class ApiClientFactory
{
    private readonly IServiceProvider _serviceProvider; // For resolving dependencies (e.g., HttpClient, IConfiguration)

    // Constructor to initialize the service provider
    public ApiClientFactory(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    // Factory method to return an appropriate API client instance based on the client type
    public IApiClient GetApiClient(string clientType)
    {
        // Return a specific implementation of IApiClient based on the input
        return clientType switch
        {
            "OpenAI" => _serviceProvider.GetRequiredService<OpenAIClient>(), // Resolve OpenAIClient from the service provider
            _ => throw new NotSupportedException($"Client type {clientType} is not supported.") // Handle unsupported client types
        };
    }
}