// Purpose: Implementation of IQuestionRepository interface. This class is responsible for fetching questions from the
// Open Trivia Database API and storing them in a list of ClientQuestion objects. It also provides a method for retrieving the list of questions.

using ServerKVIZ.Models;
using System.Net.Http;
using System.Net;
using System.Reflection;
using ServerKVIZ.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Security.AccessControl;

namespace ServerKVIZ.Repositoryes
{
    public class TriviaQuestionsRepository : IQuestionRepository
    {
        private string CacheKeyPrefix = "SessionId_";
        private readonly HttpClient _httpClient;
        private IMemoryCache allQuestions;
        
        private List<Category> categories;

        public TriviaQuestionsRepository(HttpClient httpClient, IMemoryCache questionCache)
        {
            allQuestions = questionCache;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://opentdb.com/");
            categories = new List<Category>();
           
        }
        public void RemoveQuestions(int sessionId)
        {
            
            var cacheKey = CacheKeyPrefix + sessionId;
            allQuestions.Set(cacheKey, new List<ClientQuestion>());
            allQuestions.Remove(cacheKey);
        }
        public List<Category> GetCategories()
        {
            return categories;
        }
        public async Task StoreCategories()
        {
            categories.Clear();
            var url = "https://opentdb.com/api_category.php";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<TriviaCategoryApi>(url);
                if (response.Categories == null)
                {
                    Console.WriteLine("Prilikom povlacenja kategorija sa trivije doslo je do greske");
                    
                    return;
                }

                categories = response.Categories.Select(c => new Category(c.Id, WebUtility.HtmlDecode(c.Name))).ToList();
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Pri pohrani kategorija doslo je do greske: {ex.Message}");
                categories.Clear();
            }
        }
        public async Task StoreQuestions(int sessionId, int categ, int difficulty)
        {

            
            string difficultyToString = "";
            if (difficulty == 1)
            {
                difficultyToString = "easy";
            }else if (difficulty == 2)
            {

                difficultyToString = "medium";

            }
            else
            {
                difficultyToString = "hard";
            }
             var cacheKey = CacheKeyPrefix + sessionId;
            
            var url = $"https://opentdb.com/api.php?amount=50&category={categ}&difficulty={difficultyToString}&type=multiple";
            
            try
            {
                var response = await _httpClient.GetFromJsonAsync<TriviaQuestionApi>(url);
                if (response?.Questions == null || response.ResponseCode != 0)
                {
                    Console.WriteLine($"Prilikom povlacenja pitanja doslo je do greske: {response?.ResponseCode}");
                    allQuestions.Set(cacheKey, new List<ClientQuestion>());
                    return;
                }

                // Create questions with deduplication (only once)
                var questions = response.Questions
                    .Select(q => new ClientQuestion(
                        q.GenerateId(),
                        WebUtility.HtmlDecode(q.Text),
                        WebUtility.HtmlDecode(q.CorrectAnswer),
                        new List<string> { WebUtility.HtmlDecode(q.CorrectAnswer) }
                            .Concat(q.IncorrectAnswers.Select(ia => WebUtility.HtmlDecode(ia)))
                            .OrderBy(a => Guid.NewGuid()) // Randomize correct answer position
                            .ToList(),
                        q.Category,
                        q.Difficulty
                    ))
                    .GroupBy(q => q.Text)
                    .Select(g => g.First())
                    .ToList();

                Console.WriteLine($"Storing {questions.Count} questions in cache with key: {cacheKey}");
                

                // Store questions with a specific cache duration and absolute expiration
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(2));

                allQuestions.Set(cacheKey, questions, cacheOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greska u pohrani pitanja: {ex.Message}");
                allQuestions.Set(cacheKey, new List<ClientQuestion>());
            }
        }
        public List<ClientQuestion> GetQuestions(int sessionId)
        {
            var cacheKey = CacheKeyPrefix + sessionId;
           

            if (allQuestions.TryGetValue(cacheKey, out List<ClientQuestion> questions))
            {
              
                Console.WriteLine($"Retrieved {questions.Count} questions for session {sessionId}");
                return questions;
            }

           
            return new List<ClientQuestion>();
        }
    }
}
