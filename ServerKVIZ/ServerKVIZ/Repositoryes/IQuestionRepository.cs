using ServerKVIZ.Models;

namespace ServerKVIZ.Repositoryes
{
    public interface IQuestionRepository
    {
        Task StoreQuestions(int sessionId, string categ, string dificulty);
        List<ClientQuestion> GetQuestions(int sessionId);
        public List<Category> GetCategories();
        public Task StoreCategories();
        public void RemoveQuestions(int sessionId);
    }
}
