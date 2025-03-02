using ServerKVIZ.Models;

namespace ServerKVIZ.Services
{
    public interface IPlayerServices
    {
        public Player GetPlayerById(string playerId);
    }
}
