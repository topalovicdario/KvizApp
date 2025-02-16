namespace ServerKVIZ.Models
{
    public class Players
    {
        public string NickName {  get; private set; }
        public int Score { get; private set; }
        public int Id { get; private set; }
        public string Password { get; private set; }
        public Players(int id,string nickName, string password, int score)
        {
            NickName = nickName;
            Password = password;
            Score = score;
            Id = id;


        }

       


    }
}
