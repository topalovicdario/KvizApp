using ServerKVIZ.Models;
using System.Net.Http;
using System.Net;
using System.Reflection;

namespace ServerKVIZ.Services
{
    public class TriviaQuestions : IGetQuestions
    {
        private readonly HttpClient _httpClient;
        private List<ClientQuestion> questions;

        public TriviaQuestions(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://opentdb.com/"); // Optional since base address can be set in DI as well
            questions = new List<ClientQuestion>();
        }

        public async Task StoreQuestions(string categ, string difficulty)
        {
            var url = "https://opentdb.com/api.php?amount=50&category=9&difficulty=medium&type=multiple";

            try
            {



                var response = await _httpClient.GetFromJsonAsync<TriviaOutput>(url);
                if (response?.Results == null || response.ResponseCode != 0)
                {
                    Console.WriteLine($"Invalid response: aloooooo {response?.ResponseCode}");
                    questions.Clear(); // Clear existing questions if the response is invalid
                    return;
                }

                // Populate questions list
                questions = response.Results.Select(q => new ClientQuestion(
                        q.GenerateId(),
                        WebUtility.HtmlDecode(q.Text),
                        WebUtility.HtmlDecode(q.CorrectAnswer),
                        new List<string> { WebUtility.HtmlDecode(q.CorrectAnswer) }
                            .Concat(q.IncorrectAnswers.Select(ia => WebUtility.HtmlDecode(ia)))
                            .OrderBy(a => Guid.NewGuid()) // Randomize answers
                            .ToList(),
                        q.Category,
                        q.Difficulty
                    )).ToList();


            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., network errors, deserialization issues)
                // Log the exception or handle appropriately
                Console.WriteLine($"Error fetching questions: {ex.Message}");
                questions.Clear();
            }
            questions = questions
    .GroupBy(q => q.Text)          // Grupiramo po tekstu
    .Select(g => g.First())        // Uzimamo samo prvi iz svake grupe
    .ToList();
        }
        public List<ClientQuestion> GetQuestions(string cat, string dif)
        {
            return questions;
        }
    }
}
