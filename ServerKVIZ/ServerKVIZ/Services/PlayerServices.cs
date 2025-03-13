
using ServerKVIZ.Models;
using ServerKVIZ.Repositoryes;

namespace ServerKVIZ.Services
{
    public class PlayerServices : IAuthentificatable, IPlayerServices
    {
        private IPlayerRepository playerRepository;

        public PlayerServices(IPlayerRepository playerRepository)
        {
            this.playerRepository = playerRepository;
        }
            public async Task<bool> Authentificate(string nickName, string password)
        {
            await playerRepository.StorePlayers();
            
            if (playerRepository.GetAllPlayers().Any(x => x.NickName == nickName && x.Password == password))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public Player GetPlayerById(string playerId)
        {
            
            return playerRepository.GetPlayerById(playerId);
        }
       
    
    }
}
