namespace ServerKVIZ.Models
{
    public class Player
    {
        public string NickName {  get; private set; }
        public int Score { get;  set; }
        public int Id { get; private set; }
        public string Password { get; private set; }
        public Player(int id,string nickName, string password, int score)
        {
            NickName = nickName;
            Password = password;
            Score = score;
            Id = id;


        }

       


    }
}
