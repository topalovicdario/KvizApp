using Microsoft.AspNetCore.Mvc;
using ServerKVIZ.Models;

namespace ServerKVIZ.Services
{
    public interface IGameSessionService
    {
        public int GetSessionIdForUser(string playerId);
        
        public void RemoveGameSession(string playerId);
        public GameSession GetSessionForUser(string playerId);
        public void RemovePlayerFromSession(string playerId);
        public RoundResult GetResultsAfterQuestion(string playerId);
        public bool ArePlayersDone(string playerId);
        public void SubmitAnswerToQuestion(string playerId, int questionId, int index);

        public Task<List<Category>> GetCategories();
        public Task<ClientQuestion> GetNextQuestion(string playerId);
        public bool JoinGameSession(string playerId, string player2);
        public void SetParametersOfGameSession(string playerId,int categ, int difficulty);
        public Task<GameSession> CreateGameSession( string player1Id);
        public bool IsGameOver(string playerId);

    }
}
