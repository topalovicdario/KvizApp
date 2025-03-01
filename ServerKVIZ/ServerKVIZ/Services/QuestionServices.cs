//imlementirati da sve operacije izvode po session id koji je i u question repozirotirju
//answered_questions isto u imemory cache je stavljen ali treba jos odraditi logiku za spremanje po sessionid
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MySqlX.XDevAPI;
using ServerKVIZ.Models;
using ServerKVIZ.Repositoryes;
using System.Net;

namespace ServerKVIZ.Services
{
    public class QuestionServices: IQuestionService
    {
        
        private IQuestionRepository database;
       // private IMemoryCache questionsCache; //Da li ti je ovo potrebno ? Provjeri
        private IMemoryCache answered_questions;
        private readonly string CacheKeyPrefix = "AnsweredQuestionsForSessionId_";

        public QuestionServices(IQuestionRepository database, IMemoryCache questionsCache,IMemoryCache answeredQuestions)
        {
            this.database = database;
           //this.questionsCache = questionsCache;
            answered_questions = answeredQuestions;
           

        }
        public void RemoveQuestions(int sessionId)
        {
            var cacheKey = CacheKeyPrefix + sessionId;
            answered_questions.Remove(cacheKey);
            database.RemoveQuestions(sessionId);

            //questionsCache.Remove(cacheKey);



        }

        public async Task StoreQuestions(int sessionId, string categ, string difficulty)
        {
            var cacheKey = CacheKeyPrefix + sessionId;

            await database.StoreQuestions(sessionId,categ, difficulty);  
            answered_questions.Set(cacheKey, new List<ClientQuestion>());

            // var questions = database.GetQuestions(sessionId);
            // questionsCache.Set(cacheKey, questions);  
        }

        public async Task<ActionResult<ClientQuestion>> GetNextQuestion(int sessionId)
        {
            var cacheKey = CacheKeyPrefix + sessionId;
            var allQuestions = database.GetQuestions(sessionId);

            if (allQuestions == null || !allQuestions.Any())
                return new NotFoundResult();

            
            List<ClientQuestion> answeredQuestions;
            if (!answered_questions.TryGetValue(cacheKey, out answeredQuestions))
            {
                answeredQuestions = new List<ClientQuestion>();
                answered_questions.Set(cacheKey, answeredQuestions);
            }

            var answeredIds = answeredQuestions.Select(q => q.Id).ToList();

            
            var unansweredQuestions = allQuestions
                .Where(q => !answeredIds.Contains(q.Id))
                .ToList();

            if (!unansweredQuestions.Any())
                return new NotFoundResult();  

            Random random = new Random();
            int index = random.Next(0, unansweredQuestions.Count);
            var question = unansweredQuestions[index];
            answeredQuestions.Add(question);
            answered_questions.Set(cacheKey, answeredQuestions);

            return question;
        }

        public ClientQuestion GetQuestionById(int sessionId, int questionId)
        {
            var cacheKey = CacheKeyPrefix + sessionId;
            var questions = database.GetQuestions(sessionId);
            return questions?.FirstOrDefault(q => q.Id == questionId);
        }

        public bool CheckAnswer(int sessionId, int questionId, string clientAnswer)
        {
            if (GetQuestionById(sessionId,questionId).CorrectAnswer == clientAnswer)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetCorrectAnswer(int sessionId, int questionId)
        {
            return GetQuestionById(sessionId, questionId).CorrectAnswer ;
        }
    }
}
