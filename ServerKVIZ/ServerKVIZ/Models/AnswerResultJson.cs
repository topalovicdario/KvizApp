namespace ServerKVIZ.Models
{
    public class AnswerResultJson
    {
        public bool IsCorrect { get; set; }
        public string CorrectAnswer { get; set; }
        public int PlayerScore { get; set; }
        //u slucaju da ne postoji drugi igrac (single player mode)
        
        public AnswerResultJson(bool isCorrect, string correctAnswer, int playerScore)
        {
            IsCorrect = isCorrect;
            CorrectAnswer = correctAnswer;
            PlayerScore = playerScore;
           


        }
    }
}
