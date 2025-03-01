using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ServerKVIZ.Models;

namespace ServerKVIZ.Services
{
    public class GameSessionServices
    {
        private readonly string gameSessionCacheKeyPrefix = "GameSession_";
        private readonly IQuestionService questionService;
        private readonly IMemoryCache gameSessionsCache;
        public GameSessionServices(QuestionServices questionService, IMemoryCache gameSessionsCache)
        {
            this.gameSessionsCache = gameSessionsCache;
            this.questionService = questionService;
        }
        public async Task CreateGameSession(string category, string difficulty, Player player1)
        {
            var gameSession = new GameSession(player1);
            var cacheKey = gameSessionCacheKeyPrefix + gameSession.Id;
            gameSessionsCache.Set(cacheKey, gameSession);

           
            await questionService.StoreQuestions(gameSession.Id, category, difficulty);
        }
        public void SetParametersOfGameSession(string sessionId, string categ, string difficulty)
        { 
            if (gameSessionsCache.TryGetValue<GameSession>(gameSessionCacheKeyPrefix + sessionId, out var gameSession))
            {
                gameSession.SetParametersOfGame(categ, difficulty);
                
            }
            


        }
        public bool JoinGameSession(string sessionId, Player player2)
        {
            var cacheKey = gameSessionCacheKeyPrefix + sessionId;
            if (gameSessionsCache.TryGetValue<GameSession>(cacheKey, out var gameSession))
            {
                if (gameSession.player2 == null)
                {
                    gameSession.AddPlayer(player2);
                    gameSessionsCache.Set(cacheKey, gameSession);
                    return true;
                }
            }
            return false;
        }
        public void RemoveGameSession(int sessionId)
        {   gameSessionsCache.Remove(gameSessionCacheKeyPrefix + sessionId);
            questionService.RemoveQuestions(sessionId);

        }
        public async Task<ActionResult<ClientQuestion>> GetNextQuestion(int sessionId)
        {
            return await questionService.GetNextQuestion(sessionId);
        }

        public AnswerResultJson CheckAnswerForQuestion(int playerId,int sessionId,int questionId,int index)
        {
            var cacheKey = gameSessionCacheKeyPrefix + sessionId; //treba ovo odvojit u posebnu metodu tako da bude lakse postavljanje novog nacina dodjele prefiksa ali nek ostane sad ovako
            var question = questionService.GetQuestionById(sessionId, questionId);
            
            if (question == null)
            {
                throw new ArgumentException("Nema tog pitanja u bazu");
            }
            var isCorrect = false;
            var correctAnswer = question.CorrectAnswer;
            var playerScore=0;
            
            if (gameSessionsCache.TryGetValue<GameSession>(cacheKey, out var gameSession))
            {
                if (gameSession.player1.Id == playerId)
                {
                    playerScore = gameSession.player1.Score;
                    
                    
                }
                else if (gameSession.player2.Id == playerId)
                {
                    playerScore = gameSession.player2.Score;
                    

                }
            }
           //koncept je preuzak, problem pri dodavanju vise igraca (ukoliko ostaje dualnog tipa onda je uredu ovaj pristup (1v1))

            if (question.CorrectAnswer == question.AllAnswers[index])
            {
                playerScore++;
                isCorrect = true;

            }
            else
            {
                isCorrect = false;
            }

            return new AnswerResultJson(isCorrect,correctAnswer, playerScore);



        }
        public GameSessionStatusResponse GetGameSessionStatus(int sessionId)
        {
            var cacheKey = gameSessionCacheKeyPrefix + sessionId;
            var player1Score = 0;
            var player2Score = 0;
            var questionNumber = 0;
            if (gameSessionsCache.TryGetValue<GameSession>(cacheKey, out var gameSession))
            {
                return new GameSessionStatusResponse(gameSession.player1.Score, gameSession.player2.Score, gameSession.QuestionNumber);
            }
            else
            {
                throw new ArgumentException("Nema te sesije");
            }
        }
        
    }
}
