using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MySqlX.XDevAPI;
using ServerKVIZ.Models;
using ServerKVIZ.Repositoryes;
using System.Collections.Concurrent;

namespace ServerKVIZ.Services
{
    public class GameSessionServices: IGameSessionService
    {
        private readonly IMemoryCache roundStatus;
        private readonly IDictionary<int,int> numberOfAnswers;
        private readonly string gameSessionCacheKeyPrefix = "GameSession_";
        private readonly IQuestionService questionService;
        private readonly IMemoryCache gameSessionsCache;
        private readonly ConcurrentDictionary<string, int> playerToSessionMap;
        private readonly IPlayerServices playerServices;
        public GameSessionServices(IQuestionService questionService, IMemoryCache gameSessionsCache, IPlayerServices playerServices,IMemoryCache roundStatus)
        {
            this.gameSessionsCache = gameSessionsCache;
            this.questionService = questionService;
            this.playerServices = playerServices;
            this.roundStatus = roundStatus;
            this.numberOfAnswers = new Dictionary<int,int>();

            playerToSessionMap = new ConcurrentDictionary<string, int>();
        }
        public async Task<GameSession> CreateGameSession( string  playerId)
        {
            
            
            var gameSession = new GameSession(playerServices.GetPlayerById(playerId)); //potencijalno ovdje prosljediti iz PlayerServica metodu koja ce iz baze vratiti
            //tog korisnika sa svim njegovim atributima -id itd...
            var cacheKey = gameSessionCacheKeyPrefix + gameSession.Id;

            
            gameSessionsCache.Set(cacheKey, gameSession);

           
            playerToSessionMap[playerId] = gameSession.Id;
            
            
            return gameSession;
        }
        public void SetParametersOfGameSession(string playerId, int categ, int difficulty)
        {
            
            int sessionId = GetSessionIdForUser(playerId);
            
            if (gameSessionsCache.TryGetValue<GameSession>(gameSessionCacheKeyPrefix + sessionId, out var gameSession))
            {
                questionService.RemoveQuestions(sessionId);
                gameSession.SetParametersOfGame(categ, difficulty);
                gameSession.QuestionNumber = 0;
                gameSession.player1.Score = 0;
                gameSession.player2.Score = 0;

                questionService.StoreQuestions(gameSession.Id, categ, difficulty);
                
            }
            


        }
        public bool JoinGameSession(string playerId, string player2Id)
        {
            int sessionId = GetSessionIdForUser(playerId);
            var cacheKey = gameSessionCacheKeyPrefix + sessionId;
            if (gameSessionsCache.TryGetValue<GameSession>(cacheKey, out var gameSession))
            {
                if (gameSession.player2 == null)
                {
                    gameSession.AddPlayer(playerServices.GetPlayerById(player2Id));
                    gameSessionsCache.Set(cacheKey, gameSession);
                    
                    
                    playerToSessionMap[playerServices.GetPlayerById(player2Id).NickName] = gameSession.Id;

                    return true;
                }
            }
            return false;
        }
        public void RemoveGameSession(string playerId)
        {
            int sessionId = GetSessionIdForUser(playerId);
            var cacheKey = gameSessionCacheKeyPrefix + sessionId;

            if (gameSessionsCache.TryGetValue<GameSession>(cacheKey, out var gameSession))
            {
                
                playerToSessionMap.TryRemove(gameSession.player1.NickName, out _);

                if (gameSession.player2 != null)
                {
                    playerToSessionMap.TryRemove(gameSession.player2.NickName, out _);
                }
            }

            gameSessionsCache.Remove(cacheKey);
            questionService.RemoveQuestions(sessionId);

        }
        public async Task<ClientQuestion> GetNextQuestion(string playerId)
        {
            var session = GetSessionForUser(playerId);
            
            return await questionService.GetNextQuestion(session.Id);
        }

        public void SubmitAnswerToQuestion(string playerId,int questionId,int index) //ovo samo pohrani u klasu stanja rezultata 
        {
            
            
            int sessionId = GetSessionIdForUser(playerId);
            var cacheKey = gameSessionCacheKeyPrefix +sessionId ; //treba ovo odvojit u posebnu metodu tako da bude lakse postavljanje novog nacina dodjele prefiksa ali nek ostane sad ovako
            var question = questionService.GetQuestionById(sessionId, questionId);
            if (!numberOfAnswers.ContainsKey(sessionId))
            {
                
                    numberOfAnswers[sessionId] = 0;
            }
            

           
            if (question == null)
            {
                throw new ArgumentException("Nema tog pitanja u bazu");
            }
            
             numberOfAnswers[sessionId]+=1;
            if (gameSessionsCache.TryGetValue<GameSession>(cacheKey, out var gameSession))
            {
                if (numberOfAnswers[sessionId] == 3)//popravit ovu logiku ovo negdje drugo odradit
                {
                    numberOfAnswers[sessionId]=1;
                }
                if (numberOfAnswers[sessionId] == 1)
                {
                    gameSession.QuestionNumber++;
                }
                roundStatus.Set(playerId, new RoundResult());
                roundStatus.TryGetValue<RoundResult>(playerId, out var roundResult);

                if (gameSession.player1.NickName == playerId)
                {
                    if (question.AllAnswers[index] == question.CorrectAnswer)
                    {
                        gameSession.player1.Score++;
                        roundResult.PlayerScore = gameSession.player1.Score;
                        roundResult.IsAnswerCorrect = true;
                    }
                    else
                    {
                       
                        roundResult.PlayerScore = gameSession.player1.Score;
                        roundResult.IsAnswerCorrect =false;
                    }
                    
                    
                }
                else if (gameSession.player2.NickName == playerId)
                {
                    
                    if (question.AllAnswers[index] == question.CorrectAnswer)
                    {
                        gameSession.player2.Score++;
                        roundResult.PlayerScore=gameSession.player2.Score;
                        roundResult.IsAnswerCorrect = true;
                    }
                    else
                    {
                        roundResult.PlayerScore = gameSession.player2.Score;
                        roundResult.IsAnswerCorrect = false;
                    }


                }
                Console.WriteLine("\n Score "+gameSession.player1.Score+":"+gameSession.player2.Score+ "\n");
               
                
                
                roundResult.QuestionNumber=gameSession.QuestionNumber;
                roundResult.CorrectAnswer = question.CorrectAnswer;
            }
           //koncept je preuzak, problem pri dodavanju vise igraca (ukoliko ostaje dualnog tipa onda je uredu ovaj pristup (1v1))

            
            

            



        }
        public bool ArePlayersDone(string playerId)
        {
            int sessionId = GetSessionIdForUser(playerId);
            Console.WriteLine("Odgovorilo je : "
                                                   + numberOfAnswers[sessionId]);
            return numberOfAnswers[sessionId] ==2;
        }

        public RoundResult GetResultsAfterQuestion(string playerId)//ispraviti metodu da 
        { int sessionId = GetSessionIdForUser(playerId);
            var cacheKey = gameSessionCacheKeyPrefix + sessionId;
            
            if (gameSessionsCache.TryGetValue<GameSession>(cacheKey, out var gameSession))
            {
                

                var player= playerServices.GetPlayerById(playerId);
                
                    roundStatus.TryGetValue<RoundResult>(playerId, out var roundResult);
                
                if (gameSession.player1.NickName == playerId)
                {
                    roundResult.EnemyScore=gameSession.player2.Score;
                }
                else
                {
                    roundResult.EnemyScore = gameSession.player1.Score;

                }

                //jako lose implementirana logika za dobijanje protivnikovog scora, potrebna popravka ove logike,
                //enemy score i gotov roundresult objekt treba biti odradjen u drugoj funkiciji koju cemo ovdje pozvati
                    return roundResult;
               
                    
                
               //vrati odgovor -da li je tocan odgovor, tocan odgovor, stanje scora i koje je pitanje po redu to bilo.
               //ako je prvi igrac vrati tako da enemy bude player 2 i obrnuto
            }
            else
            {
                throw new ArgumentException("Nema te sesije");
            }
        }

        public int GetSessionIdForUser(string playerId)
        {
            
            if (playerToSessionMap.TryGetValue(playerId, out var sessionId))
            {
               
                if (gameSessionsCache.TryGetValue<GameSession>(gameSessionCacheKeyPrefix + sessionId, out var gameSession))
                {
                   
                    if (gameSession.player1.NickName == playerId || (gameSession.player2 != null && gameSession.player2.NickName == playerId))
                    {
                        return gameSession.Id;
                    }
                    else
                    {
                       
                        playerToSessionMap.TryRemove(playerId, out _);
                    }
                }
                else
                {
                    
                    playerToSessionMap.TryRemove(playerId, out _);
                }
            }

           return 0;
        }
        public async Task<List<Category>> GetCategories()
        {
            return await questionService.GetCategories();
        }
        public GameSession GetSessionForUser(string playerId) //iznimno suvisno, vec imas funkciju koja vraca samo id (nekad objedini u jednu)
        {
           
            if (playerToSessionMap.TryGetValue(playerId, out var sessionId))
            {
               
                if (gameSessionsCache.TryGetValue<GameSession>(gameSessionCacheKeyPrefix + sessionId, out var gameSession))
                {

                    if (gameSession.player1.NickName == playerId || (gameSession.player2 != null && gameSession.player2.NickName == playerId))
                    {
                        return gameSession;
                    }
                    else
                    {

                        playerToSessionMap.TryRemove(playerId, out _);
                    }
                }
                else
                {

                    playerToSessionMap.TryRemove(playerId, out _);


                }
            }

            return null;
        }
        public bool IsGameOver(string playerId)
        {
            if (playerToSessionMap.TryGetValue(playerId, out var sessionId))
            {

                if (gameSessionsCache.TryGetValue<GameSession>(gameSessionCacheKeyPrefix + sessionId, out var gameSession))
                {
                    if (gameSession.QuestionNumber == 2)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void RemovePlayerFromSession(string playerId)
        {
            playerToSessionMap.TryRemove(playerId, out _);
        }
    }
}
