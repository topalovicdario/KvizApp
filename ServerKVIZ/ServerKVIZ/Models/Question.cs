using System.Text.Json.Serialization;

namespace ServerKVIZ.Models
{
    public class Question
    {public int GenerateId()
        {
            Id = Guid.NewGuid().GetHashCode();
            return Id;
        }
        public int Id { private set; get; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("question")]
        public string Text { get; set; }

        [JsonPropertyName("correct_answer")]
        public string CorrectAnswer { get; set; }

        [JsonPropertyName("incorrect_answers")]
        public List<string> IncorrectAnswers { get; set; }

        [JsonPropertyName("difficulty")]
        public string Difficulty { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

    }
}
