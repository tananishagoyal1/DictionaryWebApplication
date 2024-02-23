using System.Text.Json.Serialization;

namespace DictionaryApp.Models.DTO
{
    public class Meaning
    {
        [JsonPropertyName("partOfSpeech")]
        public string? PartOfSpeech { get; set; }

        [JsonPropertyName("definitions")]
        public List<DefinitionOfWord>? Definitions { get; set; }

        [JsonPropertyName("synonyms")]
        public List<string>? Synonyms { get; set; }

        [JsonPropertyName("Antonyms")]
        public List<string>? Antonyms { get; set; }
    }
}
