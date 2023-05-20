using Newtonsoft.Json;

namespace webapi.Params.HttpRequest
{
    public class OfferUpdateParams
    {
        [JsonProperty("offerid")]
        public int Offerid { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

    }
}
