using Newtonsoft.Json;

namespace webapi.Entities
{
    public class Push
    {
        [JsonProperty("pushid")]
        public int PushId { get; set; }

        [JsonProperty("userid")]
        public Guid UserId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("checked")]
        public bool Checked { get; set; }
    }
}
