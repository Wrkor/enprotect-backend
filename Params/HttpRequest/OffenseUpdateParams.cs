using Newtonsoft.Json;

namespace webapi.Params.HttpRequest
{
    public class OffenseUpdateParams
    {
        [JsonProperty("offenseid")]
        public int Offenseid { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

    }
}
