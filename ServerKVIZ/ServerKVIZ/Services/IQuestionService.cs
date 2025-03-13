using Microsoft.AspNetCore.Mvc;
using ServerKVIZ.Models;

namespace ServerKVIZ.Services
{
    public interface IQuestionService
    {
        public Task<List<Category>> GetCategories();
        public  Task StoreQuestions(int sessionId,int categ, int difficulty);
        public  Task<ClientQuestion> GetNextQuestion(int sessionId);
        public ClientQuestion GetQuestionById(int sessionId, int questionId);
        public bool CheckAnswer(int sessionId,int questionId, string clientAnswer);
        public String GetCorrectAnswer(int sessionId,int questionId);
        public void RemoveQuestions(int sessionId);

    }
}
