using KvizApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace KvizApp.Services
{

    public class AuthService : IAuthService
    {
        HttpClient client;
         
        public async Task<bool> AuthenticateAsync(User user)
        {
           
            try
            { client = new HttpClient();
                var url = "http://localhost:5006/Questions/authMe";
                
               
                var requestBody = new
                {
                    user.NickName,
                    user.Password

                };

               var response = await client.PostAsJsonAsync(url, requestBody);
                var responseContent = await response.Content.ReadFromJsonAsync<bool>();
                if (responseContent)
                {
                    return true;
                }
                else
                {
                    Debug.WriteLine(  " Neispravan korisnik");
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message +" Greska pri provjeri korisnika");
                return false;
            }


        }
    }
}
