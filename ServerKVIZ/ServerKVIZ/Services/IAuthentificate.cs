namespace ServerKVIZ.Services
{
    public interface IAuthentificate
    {
        bool Authentificate(string nickName, int password);
    }
}
