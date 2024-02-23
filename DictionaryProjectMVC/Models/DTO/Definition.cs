using System.Text.Json.Serialization;

namespace DictionaryApp.Models.DTO
{
    public class DefinitionOfWord
    {
        [JsonPropertyName("definition")]
        public string? Definition { get; set; }

        [JsonPropertyName("synonyms")]
        public List<string>? Synonyms { get; set; }

        [JsonPropertyName("Antonyms")]
        public List<string>? Antonyms { get; set; }


        [JsonPropertyName("example")]
        public string? Example { get; set; }
    }
}
