namespace ServerKVIZ.Services
{
    public interface IAuthentificate
    {
        Task<bool> Authentificate(string nickName, string password);
    }
}
