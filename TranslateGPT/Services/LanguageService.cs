using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System;
using MongoDB.Driver;
using MongoDB.Bson; 

using Microsoft.Extensions.Options;

public class LanguageService
{
    private readonly IMongoCollection<BsonDocument> _languagesCollection;

    public LanguageService(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var client = new MongoClient(mongoDbSettings.Value.ConnectionString);
        var database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _languagesCollection = database.GetCollection<BsonDocument>(mongoDbSettings.Value.LanguagesCollection);
    }

    public List<BsonDocument> GetLanguages()
    {
        // Fetch all the documents from the collection and return them as a list
        TestConnection();
        var languages = _languagesCollection.Find(Language => true).ToList();
        return languages;
    }

    public void TestConnection()
    {
        try
        {
            // Check if the connection works by sending a ping command
            var command = new BsonDocument("ping", 1);
            var result = _languagesCollection.Database.RunCommand<BsonDocument>(command);
            Console.WriteLine("MongoDB connection is successful!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"MongoDB connection failed: {ex.Message}");
        }
    }
}
