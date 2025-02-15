using ServerKVIZ.Models;

namespace ServerKVIZ.Services
{
    public interface IGetQuestions
    {
        Task StoreQuestions(string categ, string dificulty);
        List<ClientQuestion> GetQuestions(string categ,string dif);
    }
}
