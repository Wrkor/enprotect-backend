using Newtonsoft.Json;

namespace webapi.Params.HttpRequest
{
    public class PushParams
    {
        [JsonProperty("pushid")]
        public int[] Pushid { get; set; }
    }
}
