using Newtonsoft.Json;

namespace webapi.Params.HttpRequest
{
    public class OfferParams
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("imgs")]
        public IFormFileCollection? Imgs { get; set; }
    }
}
