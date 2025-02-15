using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace KvizApp.Services
{
    class OnlineGame : ICreatable
    {

        private HttpClient client;
        public OnlineGame()
        {
            client = new HttpClient();
        }
        public async Task<bool> CreateGame(string nicknameUser,
            string nicknameEnemy,
            string category,
            string difficulty,
            string duration)
        {
            try


            {

                var url = "http://localhost:5006/Questions/startGame";


                var requestBody = new
                {
                    nicknameUser,
                    nicknameEnemy,
                    difficulty,
                    category,
                    duration


                };

                var response = await client.PostAsJsonAsync(url, requestBody);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception e)
            {
                return false;



            }

        }
        public async Task<bool> DeleteGame()
        {
            var url = "http://localhost:5006/Questions/endGame";
            var response = await client.PostAsync(url, null);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
