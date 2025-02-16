using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ServerKVIZ.Services;

namespace ServerKVIZ.Models
{
    public class GameSession
    {
        public int Id { get; private set; }
        public string Category { get; private set; }
        public string Difficulty { get; private set; }
        public DataBaseQuestion DataBaseQuestion { get; private set; }
        public OnlineDataBase OnlineDataBase { get; private set; }
        public Players player1{ get; private set; }
        public Players player2 { get; private set; }

        public GameSession(DataBaseQuestion dataBaseQuestion)
        {
            

            Id = Guid.NewGuid().GetHashCode();
            
            DataBaseQuestion = dataBaseQuestion;
        }
        public void Init(string categ, string difficulty)
        {

            Destroy();
            Category = categ;
            Difficulty = difficulty;
        }
        public void Destroy()
        {
           DataBaseQuestion.destroy();

        }
        public async Task<ActionResult<ClientQuestion>> GetQuestion()
        {
            return await DataBaseQuestion.GetQuestion();
        }
        public async Task StoreQuestions()
        {
            await DataBaseQuestion.StoreQuestions(Category,Difficulty);
        }
        public ClientQuestion GetById(int id)
        {
            return DataBaseQuestion.GetById(id);
        }
        





        }
}
