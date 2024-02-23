using System.Text.Json.Serialization;

namespace DictionaryApp.Models.DTO
{
    public class WordData
    {
        [JsonPropertyName("word")]
        public string? Word { get; set; }

        [JsonPropertyName("phonetic")]
        public string? Phonetic { get; set; }

        [JsonPropertyName("phonetics")]
        public List<Phonetic>? Phonetics { get; set; }

        [JsonPropertyName("meanings")]
        public List<Meaning>? Meanings { get; set; }
    }
}
