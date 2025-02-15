using Microsoft.AspNetCore.Mvc;
using ServerKVIZ.Models;
using ServerKVIZ.Services;
using System.Text.Json.Serialization;

namespace ServerKVIZ.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly IServiceProvider serviceProvider;
        
        private  GameSession gameSession;

        public QuestionsController( IServiceProvider service, GameSession game)
        {
            serviceProvider = service;
            gameSession = game;

        }

        [HttpGet]
        public async Task<ActionResult<ClientQuestion>> GetQuestions()
        {
            var question = await gameSession.GetQuestion();
            if (question == null)
            {
                return NotFound("Question not found.");
            }
            Console.WriteLine("saljem ti pitanje");
            return question;
        }
        [HttpPost("startGame")]
        public async Task<ActionResult> StartGame([FromBody] CreateGameRequest request)
        {
            
            //var questionsService = serviceProvider.GetRequiredService<DataBaseQuestion>();
            //gameSession = ActivatorUtilities.CreateInstance<GameSession>(serviceProvider, request.category,request.difficulty, questionsService);
            gameSession.Init(request.Category, request.Difficulty);
            await gameSession.StoreQuestions(request.Category, request.Difficulty);

            Console.WriteLine("Game Session created with id: " + gameSession.Id +" parameters "+ gameSession.Category + " "+ request.Difficulty);
            return Ok();
        }
        [HttpPost("endGame")]
        public async Task<ActionResult> EndGame()
        {

            gameSession.Destroy();
            return Ok();
        }
        [HttpPost("checkAnswer")]
        
        public ActionResult<SendAnswerCheckQuestion> CheckAnswer([FromBody] AnswerRequest request)
        {
            Console.WriteLine($"Primljen odgovor: {request.Answer}, ID: {request.Id} Score: {request.Score}");

            var question = gameSession.GetById(request.Id);
            if (question == null)
            {
                return NotFound("Question not found.");
            }

            

            SendAnswerCheckQuestion ans = new SendAnswerCheckQuestion();
            ans.corect_ans = question.CorrectAnswer;
            if (request.Answer == 0)
            {
                ans.is_correct = false;
            }
            else
            {
                Console.WriteLine("Odgovor playera je "+ question.AllAnswers[request.Answer - 1]);
                ans.is_correct = question.CorrectAnswer==question.AllAnswers[request.Answer - 1];
            ans.enemy_score = -1;
            }
            
            if (ans.is_correct)
            {
                ans.score = request.Score+1;
            }
            
            Console.WriteLine($"Odgovor je : {ans.is_correct} {ans.corect_ans} to je tocan odg, server");
            return ans;
        }

    }
    public class AnswerRequest
    {
        public int Id { get; set; }
        public int Answer { get; set; }
            public int Score { get; set; }
        }
    public class CreateGameRequest
    {
        public string NicknameUser { get; set; }
        public string NicknameEnemy { get; set; }
        public string Category{ get; set; }
        public string Difficulty { get; set; }
        public string Duration { get; set; }
    }


    public class SendAnswerCheckQuestion
    {
        [JsonPropertyName("correctAns")]
        public string corect_ans { get; set; }
        [JsonPropertyName("is_correct")]
        public bool is_correct { get; set; }
        [JsonPropertyName("enemy_score")]
        public int enemy_score { get; set; }
        [JsonPropertyName("score")]
        public int score { get; set; }
    }
}
