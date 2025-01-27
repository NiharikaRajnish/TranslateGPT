TranslateGPT

TranslateGPT is a .NET 9 MVC application that leverages OpenAI's API to provide text translation services in multiple languages. The application allows users to input text, select a target language, and receive a translated response. MongoDB is used for managing the list of available languages, and a caching system optimizes database queries for enhanced performance.

Features

Translate text using OpenAI's GPT-based translation API.

Dynamic dropdown for selecting target languages, populated from MongoDB.

Input text persistence after submission.

Error handling and logging for improved debugging.

Memory caching for language data to reduce database reads.

Prerequisites

.NET SDK 9.0: Ensure you have the .NET 9 SDK installed.

MongoDB: The application uses MongoDB for storing language data. Make sure MongoDB is installed and running.

OpenAI API Key: Obtain an API key from OpenAI to enable translation functionality.

Setup

Step 1: Clone the Repository

git clone https://github.com/yourusername/TranslateGPT.git
cd TranslateGPT

Step 2: Configure Secrets

Obtain OpenAI API Key

Visit the OpenAI website and create an account if you don't already have one.

Navigate to the API Keys section in your OpenAI account.

Click on "Create new secret key" and copy the generated key.

Add OpenAI API Key

Use the .NET Secret Manager to store your OpenAI API key securely:

dotnet user-secrets init
dotnet user-secrets set "OpenAI:ApiKey" "<your-openai-api-key>"

Alternatively, you can add it to the appsettings.json file for development purposes (not recommended for production):

{
  "OpenAI": {
    "ApiKey": "<your-openai-api-key>"
  }
}

Step 3: Configure MongoDB

Update the appsettings.json file with your MongoDB connection string:

{
  "ConnectionStrings": {
    "MongoDB": "mongodb://localhost:27017"
  },
  "DatabaseName": "TranslateGPT"
}

Step 4: Install Dependencies

Navigate to the project directory and restore dependencies:

dotnet restore

Step 5: Run the Application

Run the application locally using the following command:

dotnet run

The application will be available at https://localhost:5001.

Key Components

Controllers

HomeController.cs: Handles core functionalities including:

Fetching the list of languages from MongoDB.

Handling user input and communicating with OpenAI's API.

Models

LanguageService: Interacts with MongoDB to retrieve and cache the list of languages.

ApiClientFactory: Creates instances of API clients for external communication.

Views

Index.cshtml: Main interface for inputting text, selecting a language, and displaying the translation.
