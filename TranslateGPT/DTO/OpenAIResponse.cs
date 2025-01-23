namespace TranslateGPT.DTOs
{
    public class OpenAIResponse
{
    // Ensure Choices is a list of Choice objects
    public List<Choice> Choices { get; set; }
}

public class Choice
{
    // Ensure the Message is correctly defined
    public Message Message { get; set; }
}

public class Message
{
    // Define the Content property
    public string Content { get; set; }
}

}