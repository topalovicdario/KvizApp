using System.Text.Json.Serialization;

namespace ServerKVIZ.Models
{
    public class TriviaQuestionApi
    {
        [JsonPropertyName("response_code")]
        public int ResponseCode { get; set; }
        [JsonPropertyName("results")]
        public required List<Question> Questions { get; set; }
    }
}
