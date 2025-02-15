using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KvizApp.Models
{
    public class JsonServerAnswerQuestion
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
