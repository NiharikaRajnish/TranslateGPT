using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using TranslateGPT.Models;
using TranslateGPT.DTOs;
using Microsoft.Extensions.Options; 
using MongoDB.Driver; 
using System.Collections.Generic; 
using Microsoft.Extensions.Caching.Memory; 


namespace TranslateGPT.Controllers
{
    public class HomeController : Controller
    {
        private readonly LanguageService _languageService;
        private readonly ILogger<HomeController> _logger;
        private readonly ApiClientFactory _apiClientFactory;
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly IMemoryCache _memoryCache;

        public HomeController(
            ApiClientFactory apiClientFactory,
            LanguageService languageService,
            IHttpClientFactory httpClientFactory,
            ILogger<HomeController> logger,
            IMemoryCache memoryCache)
        {
            _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
            _languageService = languageService ?? throw new ArgumentNullException(nameof(languageService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

        }

        // Helper method to fetch and return the list of languages
private List<string> GetLanguageNames()
{
    // Check if the languages are in the cache
    if (!_memoryCache.TryGetValue("Languages", out List<string> languages))
    {
        try
        {
            // Fetch languages from the database via LanguageService
            var bsonLanguages = _languageService.GetLanguages(); // This returns a list of BsonDocument
            
            // Convert the BsonDocument to List<string> by selecting the "Name" field
            languages = bsonLanguages
                .Select(l => l["Name"].AsString) // Convert BsonDocument to string by extracting the "Name" field
                .ToList();

            // Set the languages in the cache with a 1-hour expiration (can be adjusted)
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));

            _memoryCache.Set("Languages", languages, cacheEntryOptions);
        }
        catch (Exception ex)
        {
            // Log the error and return an empty list in case of an exception
            _logger.LogError(ex, "Error fetching languages from MongoDB.");
            languages = new List<string>(); // Return an empty list if an error occurs
        }
    }

    // Return the cached or fetched languages
    return languages;
}

        public IActionResult Index()
        {
            // Fetch and pass the list of languages to the view
            var languageNames = GetLanguageNames();
            ViewBag.Languages = new SelectList(languageNames);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OpenAIGPT(string query, string selectedLanguage)
        {
            if (string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(selectedLanguage))
            {
                   
                 // Indicate that the query is invalid
                ViewBag.IsQueryInvalid = true;

                // Fetch and pass the list of languages back to the view
                var languageNames = GetLanguageNames();
                ViewBag.Languages = new SelectList(languageNames);

                return View("Index");
            }

            try
            {
                // Use the factory to get an instance of the OpenAI API client
                var apiClient = _apiClientFactory.GetApiClient("OpenAI");

                // Get the translation result from the API
                var translationResult = await apiClient.Translate(query, selectedLanguage);

                if (string.IsNullOrEmpty(translationResult))
                {
                    _logger.LogError("Translation failed.");
                    ViewBag.ErrorMessage = "An error occurred during translation.";
                }
                else
                {
                    ViewBag.IsQueryInvalid = false;

                    ViewBag.Result = translationResult;
                }
                ViewBag.InputText = query;
                 ViewBag.IsQueryInvalid = false;

                // Fetch and pass the list of languages back to the view
                var languageNames = GetLanguageNames();
                ViewBag.Languages = new SelectList(languageNames);

                return View("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the translation.");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }
    }
}
