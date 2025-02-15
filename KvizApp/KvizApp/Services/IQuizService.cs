
using KvizApp.Models;

namespace KvizApp.Services
{
    public interface IQuizService
    {
        Task <Question> DohvatiPitanje();
        Task<JsonServerAnswerQuestion> ProvjeriOdgovor(int broj, int id,int score);
    }
}
