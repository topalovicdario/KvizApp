using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Org.BouncyCastle.Asn1.BC;
using ServerKVIZ.Repositoryes;
using ServerKVIZ.Services;

namespace ServerKVIZ.Models
{
    public class GameSession
    {
        public int Id { get; set; }
        public int Category { get;  set; }
        public int Difficulty { get; set; }
        
        public int QuestionNumber { get; set; }

        public Player player1{ get;  set; }
        public Player player2 { get;  set; }



        public GameSession(Player player)
        {
            player1 = player;
            

            Id = Guid.NewGuid().GetHashCode();

            
        }
        public void SetParametersOfGame(int category, int difficulty)
        {   QuestionNumber = 0;
            Category = category;
            Difficulty = difficulty;
        }
        public void AddPlayer(Player player)
        {
            player2 = player;
        }   


        /*
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
        

        */



    }
}
