using KvizApp.Models;

namespace KvizApp.Services
{
    public interface IAuthService
    {
        Task<bool> AuthenticateAsync(User user);
    }
}
