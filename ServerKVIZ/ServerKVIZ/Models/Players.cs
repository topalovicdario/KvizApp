namespace ServerKVIZ.Models
{
    public class Players
    {
        public string NickName {  get; private set; }
        public int Score { get; private set; }
        public int Id { get; private set; }
        public int Password { get; private set; }
        public Players(string nickName, int password)
        {
            NickName = nickName;
            Password = password;
            Score = 0;
            Id = Guid.NewGuid().GetHashCode();
        }

       


    }
}
