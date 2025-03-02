namespace ServerKVIZ.Models
{
    public class RoundResult
    {
        public int PlayerScore { get; set; }
        public int EnemyScore { get; set; }
        public string CorrectAnswer {  get; set; }
        public bool IsAnswerCorrect {  get; set; }
        public int QuestionNumber { get; set; }
        public RoundResult(int playerScore, int enemyScore, int questionNumber,string correctAnswer,bool isAnswerCorrect)
        {
            PlayerScore = playerScore;
            EnemyScore = enemyScore;
            QuestionNumber = questionNumber;
            CorrectAnswer = correctAnswer;
            IsAnswerCorrect = isAnswerCorrect;

        }
    }
}
