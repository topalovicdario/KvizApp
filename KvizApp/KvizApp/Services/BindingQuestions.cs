


using KvizApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace KvizApp.Services
{
    public class QuizService : IQuizService
    {
        public Question _pitanja;
        JsonServerAnswerQuestion answer;
        public QuizService()
        {
            _pitanja = new Question(0,"","",null,"","");

        }
        public async Task  DohvatiPitanjeApi()
        {
            try


            {
                
                var url = $"http://localhost:5006/questions";
                
                HttpClient _client = new HttpClient();
                var response = await _client.GetFromJsonAsync<Question>(url);
                
                _pitanja = response;
            }
            catch (Exception e)
            {
                _pitanja = null;
                
                
            }
        }

        public async Task<Question> DohvatiPitanje()
        {
            Debug.WriteLine("ulazim u metodu koja poziva api");
            await DohvatiPitanjeApi();
            Debug.WriteLine("api proso");
            return _pitanja;

           
        }
        public async Task<JsonServerAnswerQuestion> ProvjeriOdgovor(int broj, int id,int score)
        {
           await ProvjeriOdgovorApi(broj, id,score);
            return answer;
        }
        public async Task ProvjeriOdgovorApi(int broj, int ida,int score1)
        {
            try
            {
                var url = "http://localhost:5006/Questions/checkAnswer";
                HttpClient _client = new HttpClient();
                Debug.WriteLine("answer: " + broj +" id: "+ida);
                var requestBody = new
                {
                    id = ida,
                    answer = broj,
                    score = score1
                    
                };

                var response = await _client.PostAsJsonAsync(url, requestBody);
                answer = await response.Content.ReadFromJsonAsync<JsonServerAnswerQuestion>();
                Debug.WriteLine("kupim iz server" + answer.corect_ans);
            }
            catch (Exception e)
            {
                answer = new JsonServerAnswerQuestion();
                Console.WriteLine($"Error: {e.Message}");
            }

        }
    }
}
