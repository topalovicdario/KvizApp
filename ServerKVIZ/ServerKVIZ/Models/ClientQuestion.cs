namespace ServerKVIZ.Models
{
    public class ClientQuestion
    {
       
       public int Id { set; get; }
            public string Text { get; set; }
            public string CorrectAnswer { get; set; }
            public List<string> AllAnswers { get; set; } // Kombinira correct + incorrect
           public string Category { get; set; }
        public string Difficulty { get; set; }
        public ClientQuestion(int id,string text, string correctAnswer, List<string> allAnswers,string cat,string dif)
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
