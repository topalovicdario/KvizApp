using System.Text.Json.Serialization;

namespace ServerKVIZ.Models
{
    public class RoundResult
    {
        [JsonPropertyName("PlayerScore")]
        public int PlayerScore { get; set; }
        [JsonPropertyName("EnemyScore")]
        public int EnemyScore { get; set; }
        [JsonPropertyName("CorrectAnswer")]
        public string CorrectAnswer { get; set; }
        [JsonPropertyName("IsAnswerCorrect")]
        public bool IsAnswerCorrect { get; set; }
        [JsonPropertyName("QuestionNumber")]
        public int QuestionNumber { get; set; }
        public RoundResult(int playerScore, int enemyScore, int questionNumber, string correctAnswer, bool isAnswerCorrect)
        {
            PlayerScore = playerScore;
            EnemyScore = enemyScore;
            QuestionNumber = questionNumber;
            CorrectAnswer = correctAnswer;
            IsAnswerCorrect = isAnswerCorrect;

        }
        public RoundResult() { }
    }
}
