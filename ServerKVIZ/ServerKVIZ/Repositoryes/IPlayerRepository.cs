using ServerKVIZ.Models;

namespace ServerKVIZ.Repositoryes
{
    public interface IPlayerRepository
    {
        public List<Player> GetAllPlayers();
        public  Task StorePlayers();
        public Player GetPlayerByIndex(int index);
        public Player GetPlayerById(string playerId);//glup si zbog ovog, getbyname treba
    }
}