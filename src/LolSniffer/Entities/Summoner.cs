using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LolSniffer.Entities
{
    public class Summoner
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("accountId")]
        public string AccountId { get; set; }

        [JsonPropertyName("puuid")]
        public string Puuid { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("profileIconId")]
        public int ProfileIconId { get; set; }

        [JsonPropertyName("revisionDate")]
        public long RevisionDate { get; set; }

        [JsonPropertyName("summonerLevel")]
        public int SummonerLevel { get; set; }
    }
}
