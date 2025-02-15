using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KvizApp.Models
{
    public class Question
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("correctAnswer")]
        public string CorrectAnswer { get; set; }

        [JsonPropertyName("allAnswers")]
        public List<string> AllAnswers { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("difficulty")]
        public string Difficulty { get; set; }

        public Question() { } // Default constructor for deserialization

        public Question(int id, string text, string correctAnswer, List<string> allAnswers, string cat, string dif)
        {
            Id = id;
            Text = text;
            CorrectAnswer = correctAnswer;
            AllAnswers = allAnswers;
            Category = cat;
            Difficulty = dif;
        }
    }
}
