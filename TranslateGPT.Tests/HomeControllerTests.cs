namespace TranslateGPT.Tests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using System.Threading.Tasks;
using TranslateGPT.Controllers;
using Xunit;
using Microsoft.Extensions.Logging;
using TranslateGPT.Services; // Replace with correct namespace if ApiClientFactory is in a different file
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace TranslateGPT.Tests
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly Mock<ApiClientFactory> _mockApiClientFactory;
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly HomeController _controller;

        private readonly List<string> _mostUsedLanguages = new List<string>
        {
            "English", "Mandarin Chinese", "Spanish", "Hindi", "Arabic", "Bengali", "Portuguese",
            "Russian", "Japanese", "French", "German", "Urdu", "Italian", "Indonesian", "Vietnamese",
            "Turkish", "Korean", "Tamil", "Albanian"
        };

        public HomeControllerTests()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _mockApiClientFactory = new Mock<ApiClientFactory>();
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();

            _controller = new HomeController(
                _mockApiClientFactory.Object,
                _mockLogger.Object,
                _mockHttpClientFactory.Object);
        }

        [Fact]
        public void Index_Returns_View_With_Languages()
        {
            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            var selectList = result?.ViewData["Languages"] as SelectList;
            Assert.NotNull(selectList);
            Assert.Equal(_mostUsedLanguages.Count, selectList?.Count);
        }

        [Fact]
        public async Task OpenAIGPT_Returns_View_With_Translation_Result_When_Valid_Input()
        {
            // Arrange
            string query = "Hello";
            string selectedLanguage = "Spanish";
            var mockApiClient = new Mock<ITranslationApiClient>();
            mockApiClient.Setup(client => client.Translate(query, selectedLanguage)).ReturnsAsync("Hola");

            _mockApiClientFactory.Setup(factory => factory.GetApiClient("OpenAI")).Returns(mockApiClient.Object);

            // Act
            var result = await _controller.OpenAIGPT(query, selectedLanguage) as ViewResult;

            // Assert
            Assert.Equal("Hola", result?.ViewData["Result"]);
            Assert.Null(result?.ViewData["ErrorMessage"]);
        }

        [Fact]
        public async Task OpenAIGPT_Returns_Error_Message_When_Query_Is_Empty()
        {
            // Arrange
            string query = "";
            string selectedLanguage = "Spanish";

            // Act
            var result = await _controller.OpenAIGPT(query, selectedLanguage) as ViewResult;

            // Assert
            Assert.Equal("Please provide both a query and a selected language.", result?.ViewData["ErrorMessage"]);
        }

        [Fact]
        public async Task OpenAIGPT_Returns_Error_Message_When_Translation_Fails()
        {
            // Arrange
            string query = "Hello";
            string selectedLanguage = "Spanish";
            var mockApiClient = new Mock<ITranslationApiClient>();
            mockApiClient.Setup(client => client.Translate(query, selectedLanguage)).ReturnsAsync("");

            _mockApiClientFactory.Setup(factory => factory.GetApiClient("OpenAI")).Returns(mockApiClient.Object);

            // Act
            var result = await _controller.OpenAIGPT(query, selectedLanguage) as ViewResult;

            // Assert
            Assert.Equal("An error occurred during translation.", result?.ViewData["ErrorMessage"]);
        }

        [Fact]
        public async Task OpenAIGPT_Returns_Error_Message_When_Exception_Occurs()
        {
            // Arrange
            string query = "Hello";
            string selectedLanguage = "Spanish";
            var mockApiClient = new Mock<ITranslationApiClient>();
            mockApiClient.Setup(client => client.Translate(query, selectedLanguage)).ThrowsAsync(new System.Exception("Unexpected error"));

            _mockApiClientFactory.Setup(factory => factory.GetApiClient("OpenAI")).Returns(mockApiClient.Object);

            // Act
            var result = await _controller.OpenAIGPT(query, selectedLanguage) as ViewResult;

            // Assert
            Assert.Equal("An unexpected error occurred. Please try again later.", result?.ViewData["ErrorMessage"]);
        }
    }
}
