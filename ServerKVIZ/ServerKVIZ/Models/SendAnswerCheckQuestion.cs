using System.Text.Json.Serialization;

//trenutno radim na izmjeni ovog kontrolera, zelim da ga ukinem i da sve sto se tice pitanja i odgovora prebacim u GameHub 
//koji radi sa SignalR protokolom i da sve to radi u realnom vremenu 

namespace ServerKVIZ.Models
{
    public class SendAnswerCheckQuestion
    {
        [JsonPropertyName("correctAns")]
        public string corect_ans { get; set; }
        [JsonPropertyName("is_correct")]
        public bool is_correct { get; set; }
        [JsonPropertyName("enemy_score")]
        public int enemy_score { get; set; }
        [JsonPropertyName("score")]
        public int score { get; set; }
    }
}
