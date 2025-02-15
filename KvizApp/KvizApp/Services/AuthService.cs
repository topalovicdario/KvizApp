using KvizApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvizApp.Services
{

    public class AuthService : IAuthService
    {
        public async Task<bool> AuthenticateAsync(User user)
        {
            // Ovdje dodajte stvarnu autentikacijsku logiku
            await Task.Delay(10); // Simulacija API poziva
            return user.NickName == "admin" && user.Password == "1234";
        }
    }
}
