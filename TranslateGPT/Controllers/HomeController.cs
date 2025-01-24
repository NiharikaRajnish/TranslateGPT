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

    public HomeController(LanguageService languageService, ILogger<HomeController> logger)
    {
        _languageService = languageService ?? throw new ArgumentNullException(nameof(languageService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IActionResult Index()
    {
        try
        {
            // Fetch languages from MongoDB using the LanguageService
            var languages = _languageService.GetLanguages(); // Implement GetLanguages in LanguageService
             
    // Convert BsonDocument to a List<string> (example: getting "Name" field from each document)
        var languageNames = languages.Select(l => l["Name"].AsString).ToList();

        ViewBag.Languages = new SelectList(languageNames);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching languages from MongoDB.");
            ViewBag.ErrorMessage = "Failed to load languages.";
        }
        
        return View();
    }

}

}
