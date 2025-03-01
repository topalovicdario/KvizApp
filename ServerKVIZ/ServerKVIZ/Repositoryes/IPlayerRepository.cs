using ServerKVIZ.Models;

namespace ServerKVIZ.Repositoryes
{
    public interface IPlayerRepository
    {
        public List<Player> GetAllPlayers();
        public  Task StorePlayers();
        public Player GetPlayerByIndex(int index);
    }
}