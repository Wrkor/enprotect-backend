using Newtonsoft.Json;

namespace webapi.Params.HttpResponse
{
    public class AccessesParams
    {
        [JsonProperty("accesses")]
        public string[] Accesses { get; set; }
    }
}
