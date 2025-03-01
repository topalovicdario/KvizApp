// Purpose: Implementation of IQuestionRepository interface. This class is responsible for fetching questions from the
// Open Trivia Database API and storing them in a list of ClientQuestion objects. It also provides a method for retrieving the list of questions.
//treba implementirati da se pitanja spremaju u i memory cache ili distriburianom ako smatram da ram moze biti potpuno zaizet i da se dohvacaju iz cachea po sessionId
using ServerKVIZ.Models;
using System.Net.Http;
using System.Net;
using System.Reflection;
using ServerKVIZ.Services;
using Microsoft.Extensions.Caching.Memory;

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
            _httpClient.BaseAddress = new Uri("https://opentdb.com/"); // Optional since base address can be set in DI as well
           
        }
        public void RemoveQuestions(int sessionId)
        {
            var cacheKey = CacheKeyPrefix + sessionId;
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
        public async Task StoreQuestions(int sessionId, string categ, string difficulty)
        {   
            
            List<ClientQuestion> questions= new List<ClientQuestion>();
             var cacheKey = CacheKeyPrefix + sessionId;

            var url = "https://opentdb.com/api.php?amount=50&category=9&difficulty=medium&type=multiple";

            try
            {
                var response = await _httpClient.GetFromJsonAsync<TriviaQuestionApi>(url);
                if (response?.Questions == null || response.ResponseCode != 0)
                {
                    Console.WriteLine($"Prilikom povlacenja pitanja doslo je do greske: {response?.ResponseCode}");
                    questions.Clear(); 
                    return;
                }

               
                questions = response.Questions.Select(q => new ClientQuestion(
                        q.GenerateId(),
                        WebUtility.HtmlDecode(q.Text),
                        WebUtility.HtmlDecode(q.CorrectAnswer),
                        new List<string> { WebUtility.HtmlDecode(q.CorrectAnswer) }
                            .Concat(q.IncorrectAnswers.Select(ia => WebUtility.HtmlDecode(ia)))
                            .OrderBy(a => Guid.NewGuid()) //randomizacija odgovora, tako da tocan odgovor u listi mijenja mjesto svaki put
                            .ToList(),
                        q.Category,
                        q.Difficulty
                    )).ToList();
                allQuestions.Set(cacheKey, questions);

            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Greska u pohrani pitanja: {ex.Message}");
                questions.Clear();
            }
            questions = questions
                 .GroupBy(q => q.Text)         
                 .Select(g => g.First())        
                 .ToList();
        }
        public List<ClientQuestion> GetQuestions(int sessionId)
        {
            var cacheKey = CacheKeyPrefix + sessionId;
            return allQuestions.Get<List<ClientQuestion>>(cacheKey).ToList();
        }
    }
}
