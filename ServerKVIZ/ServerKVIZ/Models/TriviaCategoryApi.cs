using System.Text.Json.Serialization;

namespace ServerKVIZ.Models
{
    public class TriviaCategoryApi
    {

       
        [JsonPropertyName("trivia_categories")]
        public required List<Category> Categories { get; set; }
    }
  
}
