using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvizApp.Models
{
    public class User
    {
        public string NickName { get; set; }
        public string Password { get; set; }
        public int Score { get; set; }
        public User(string nickName, string password)
        {
            NickName = nickName;
            Password = password;
            Score = 0;
        }
        public User()
        {
            Score = 0;
            NickName = string.Empty;
            Password = string.Empty;
        }

    }
}
