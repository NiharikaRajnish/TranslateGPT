using System;
using System.Collections.Generic;

namespace TranslateGPT.DTOs
{
    /// <summary>
    /// Represents the response from the OpenAI API.
    /// </summary>
    public class OpenAIResponse
    {
        private List<Choice> _choices = new List<Choice>(); // Encapsulation

        /// <summary>
        /// Gets or sets the list of choices returned by the API.
        /// Ensures the list is not null or empty.
        /// </summary>
        public List<Choice> Choices
        {
            get => _choices;
            set
            {
                if (value == null || value.Count == 0)
                {
                    throw new ArgumentException("Choices cannot be null or empty.");
                }
                _choices = value;
            }
        }

        /// <summary>
        /// Constructor to initialize the OpenAIResponse object.
        /// </summary>
        public OpenAIResponse()
        {
            _choices = new List<Choice>();
        }
    }

    /// <summary>
    /// Represents a single choice in the OpenAI API response.
    /// </summary>
    public class Choice
    {
        private Message _message; // Encapsulation

        /// <summary>
        /// Gets or sets the message associated with this choice.
        /// Ensures the message is not null.
        /// </summary>
        public Message Message
        {
            get => _message;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Message cannot be null.");
                }
                _message = value;
            }
        }

        /// <summary>
        /// Constructor to initialize the Choice object with a specified message.
        /// </summary>
        /// <param name="message">The message associated with the choice.</param>
        public Choice(Message message)
        {
            _message = message ?? throw new ArgumentNullException(nameof(message), "Message cannot be null.");
        }

        /// <summary>
        /// Parameterless constructor with default initialization.
        /// </summary>
        public Choice()
        {
            _message = new Message("Default message");
        }
    }

    /// <summary>
    /// Represents a message in the OpenAI API response.
    /// </summary>
    public class Message
    {
        private string _content; // Encapsulation

        /// <summary>
        /// Gets or sets the content of the message.
        /// Ensures the content is not null or empty.
        /// </summary>
        public string Content
        {
            get => _content;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Content cannot be null or whitespace.");
                }
                _content = value;
            }
        }

        /// <summary>
        /// Constructor to initialize the Message object.
        /// </summary>
        /// <param name="content">The content of the message.</param>
        public Message(string content)
        {
            Content = content; // Use the property to ensure validation.
        }
    }
}
