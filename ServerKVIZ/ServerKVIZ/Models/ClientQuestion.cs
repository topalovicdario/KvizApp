using System.Text.Json.Serialization;

namespace ServerKVIZ.Models
{
    public class ClientQuestion
    {
        [JsonPropertyName("Id")]
        public int Id { get; set; }

        [JsonPropertyName("Text")]
        public string Text { get; set; }

        [JsonPropertyName("CorrectAnswer")]
        public string CorrectAnswer { get; set; }

        [JsonPropertyName("AllAnswers")]
        public List<string> AllAnswers { get; set; } // Kombinira correct + incorrect

        [JsonPropertyName("Category")]
        public string Category { get; set; }

        [JsonPropertyName("Difficulty")]
        public string Difficulty { get; set; }

        public ClientQuestion(int id, string text, string correctAnswer, List<string> allAnswers, string cat, string dif)
        {
            Id = id;
            Text = text;
            CorrectAnswer = correctAnswer;
            AllAnswers = allAnswers;
            Category = cat;
            Difficulty = dif;
        }
        public ClientQuestion() { }
    }
}
