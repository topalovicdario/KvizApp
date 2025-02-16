using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ServerKVIZ.Services;
using System.Net;

namespace ServerKVIZ.Models
{
    public class DataBaseQuestion
    {
        private string CacheKey => "Questions";
        private IGetQuestions database;
        private IMemoryCache _questionsCache;
        private HashSet<int> answered_questions;
        private static readonly ThreadLocal<Random> _random = new ThreadLocal<Random>(() => new Random());

        public DataBaseQuestion(IGetQuestions database, IMemoryCache questionsCache)
        {
            this.database = database;
            _questionsCache = questionsCache;
            answered_questions = new HashSet<int>();
            // Load initial set of questions
            
        }
        public void destroy()
        {
            answered_questions.Clear();
            _questionsCache.Remove(CacheKey);

        }

        public async Task StoreQuestions(string categ, string difficulty)
        {

            await database.StoreQuestions(categ, difficulty);  // Await the async method
           
       
            var questions = database.GetQuestions(categ, difficulty);
            _questionsCache.Set(CacheKey, questions);  // Cache the questions
        }

        public async Task<ActionResult<ClientQuestion>> GetQuestion()
        {

            var allQuestions = _questionsCache.Get<List<ClientQuestion>>(CacheKey)

                .ToList();

            if (!allQuestions.Any()) return new NotFoundResult();  // If no questions are found

            int id; Console.WriteLine("pokusavam nac pitanje");
            Random random = new Random();
            do
            {
                id = random.Next(0, allQuestions.Count);

                id = allQuestions[id].Id;
                Console.WriteLine(id);

            } while (answered_questions.Contains(id));

            Console.WriteLine("naso pitanje");
            answered_questions.Add(id);  // Mark question as answered
            return  allQuestions.FirstOrDefault(q => q.Id == id);
        }

        public ClientQuestion GetById(int id)
        {
            var questions = _questionsCache.Get<List<ClientQuestion>>(CacheKey);
            return questions?.FirstOrDefault(q => q.Id == id);
        }
    }
}
