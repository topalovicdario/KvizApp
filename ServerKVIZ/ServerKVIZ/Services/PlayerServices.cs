
using ServerKVIZ.Models;
using ServerKVIZ.Repositoryes;

namespace ServerKVIZ.Services
{
    public class PlayerServices : IAuthentificatable
    {
        private IPlayerRepository playerRepository;

        public PlayerServices(IPlayerRepository playerRepository)
        {
            this.playerRepository = playerRepository;
        }
            public async Task<bool> Authentificate(string nickName, string password)
        {
            await playerRepository.StorePlayers();
            for (int i = 0; i < playerRepository.GetAllPlayers().Count; i++)
            {
                Console.WriteLine(i + 1 + " " + playerRepository.GetPlayerByIndex(i).NickName + " " + playerRepository.GetPlayerByIndex(i).Password);
            }
            if (playerRepository.GetAllPlayers().Any(x => x.NickName == nickName && x.Password == password))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    
    }
}
