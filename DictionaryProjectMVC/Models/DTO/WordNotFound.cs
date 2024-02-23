using System.Text.Json.Serialization;

namespace DictionaryApp.Models.DTO
{
    public class WordNotFound
    {

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("resolution")]
        public string? Resolution { get; set; }

    }
}
