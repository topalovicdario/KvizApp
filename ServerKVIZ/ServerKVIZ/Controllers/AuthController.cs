using Microsoft.AspNetCore.Mvc;
using ServerKVIZ.Models;
using ServerKVIZ.Services;
using System;
using System.Text.Json.Serialization;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace ServerKVIZ.Controllers
{[ApiController]
        [Route("[controller]")]
    public class AuthController
    {
        private OnlineDataBase onlineDataBase;

        public AuthController(OnlineDataBase onlineDataBase)
        {
            this.onlineDataBase = onlineDataBase;
        }

        [HttpPost("authMe")]
        public async Task<bool> AuthentificateUser([FromBody] AuthRequest request)
        {
            if (await onlineDataBase.Authentificate(request.Nickname, request.Password))
            {
                return true;
            }
            else
            {
                return false;
            }


        }
        public class AuthRequest
        {
            public string Nickname { get; set; }
            public string Password { get; set; }
        }
    } 


    }
