//trenutno radim na izmjeni ovog kontrolera, zelim da ga ukinem i da sve sto se tice pitanja i odgovora prebacim u GameHub 
//koji radi sa SignalR protokolom i da sve to radi u realnom vremenu 

namespace ServerKVIZ.Models
{
    public class AnswerRequest
    {
        public int Id { get; set; }
        public int Answer { get; set; }
        public int Score { get; set; }
    }
}
