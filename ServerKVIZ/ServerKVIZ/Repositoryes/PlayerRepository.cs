
//Purpose: Implementing IPlayerRepository interface and methods for storing and getting players from database that is stored 
//in online database managed by Aiven.


using Microsoft.Extensions.Caching.Memory;
using MySql.Data.MySqlClient;
using ServerKVIZ.Models;
using ServerKVIZ.Services;
using System.Collections.Concurrent;


namespace ServerKVIZ.Repositoryes
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly IConfiguration configurationOfDataBaseConnection;
        private List<Player> players;
       
        
        public PlayerRepository(IConfiguration configuration)
        {
            configurationOfDataBaseConnection = configuration;
            players = new List<Player>();
            
        }
        public List<Player> GetAllPlayers()
        {
            return players;
        }
        public Player GetPlayerByIndex(int index)
        {
            return players[index];
        }
        public Player GetPlayerById(string playerId)
        {
            return players.Where(p => p.NickName == playerId).FirstOrDefault() ;
        }
        public async Task StorePlayers()
        {


            string connectionString = String.Empty;
            try
            {
                    connectionString = configurationOfDataBaseConnection.GetConnectionString("DefaultConnection");
            }
            catch(Exception ex)
            {
                Console.WriteLine($" Prilikom dohvacanja podataka konfiguracije nastao je problem : {ex.Message}");
            }
           
            

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM users"; 

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Player igrac = new Player(
                                    reader.GetInt32("idUsers"), 
                                    reader.GetString("Nickname"), 
                                    reader.GetString("Password"), 
                                    reader.GetInt32("score"));

                                players.Add(igrac);
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    
                    Console.WriteLine($" Prilikom pohranjivanja igraca u lokalnu memoriju nastala je greska : {ex.Message}");
                }
            }

        }
        
    /*    public async Task<bool> Authentificate(string nickName, string password)
        {
            await StorePlayers();
            for (int i = 0; i < players.Count; i++)
            {
                Console.WriteLine(i + 1 + " " + players[i].NickName + " " + players[i].Password);
            }
            if (players.Any(x => x.NickName == nickName && x.Password == password))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    */
        }

}





