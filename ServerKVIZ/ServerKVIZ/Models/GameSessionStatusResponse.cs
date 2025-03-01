namespace ServerKVIZ.Models
{
    public class GameSessionStatusResponse
    {
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }

        public int QuestionNumber { get; set; }
        public GameSessionStatusResponse(int player1Score, int player2Score, int questionNumber)
        {
            Player1Score = player1Score;
            Player2Score = player2Score;
            QuestionNumber = questionNumber;
        }
    }
}
