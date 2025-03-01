namespace ServerKVIZ.Services
{
    public interface IAuthentificatable
    {
        Task<bool> Authentificate(string nickName, string password);
    }
}
