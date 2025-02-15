using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvizApp.Services
{
    public interface ICreatable
    {
         Task<bool> CreateGame(string nickname_user,
            string nickname_enemy,
            string category,
            string difficulty,
            string duration);
    }
}
