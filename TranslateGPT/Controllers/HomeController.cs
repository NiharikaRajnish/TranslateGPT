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

namespace TranslateGPT.Controllers
{
    public class HomeController : Controller
    {
        private readonly LanguageService _languageService;
        private readonly ILogger<HomeController> _logger;
        private readonly ApiClientFactory _apiClientFactory;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(
            ApiClientFactory apiClientFactory,
            LanguageService languageService,
            IHttpClientFactory httpClientFactory,
            ILogger<HomeController> logger)
        {
            _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
            _languageService = languageService ?? throw new ArgumentNullException(nameof(languageService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        // Helper method to fetch and return the list of languages
        private List<string> GetLanguageNames()
        {
            try
            {
                var languages = _languageService.GetLanguages(); // Implement GetLanguages in LanguageService
                return languages.Select(l => l["Name"].AsString).ToList(); // Convert BsonDocument to List<string>
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching languages from MongoDB.");
                return new List<string>(); // Return an empty list if an error occurs
            }
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
                _logger.LogWarning("Query or language selection is empty.");
                return BadRequest("Please provide both a query and a selected language.");
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
                    ViewBag.Result = translationResult;
                }

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
