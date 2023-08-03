using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LolSniffer.Entities
{
    public class LeagueEntry
    {
        [JsonPropertyName("queueType")]
        public string QueueType { get; set; }

        [JsonPropertyName("wins")]
        public int Wins { get; set; }

        [JsonPropertyName("losses")]
        public int Losses { get; set; }
    }

}
