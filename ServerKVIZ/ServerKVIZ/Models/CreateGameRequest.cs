//trenutno radim na izmjeni ovog kontrolera, zelim da ga ukinem i da sve sto se tice pitanja i odgovora prebacim u GameHub 
//koji radi sa SignalR protokolom i da sve to radi u realnom vremenu 

namespace ServerKVIZ.Models
{
    public class CreateGameRequest
    {
        public string NicknameUser { get; set; }
        public string NicknameEnemy { get; set; }
        public string Category { get; set; }
        public string Difficulty { get; set; }
        public string Duration { get; set; }
    }
}
