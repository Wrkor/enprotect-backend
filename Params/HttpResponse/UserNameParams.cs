using Newtonsoft.Json;

namespace webapi.Params.HttpResponse
{
    public class UserNameParams
    {
        [JsonProperty("sname")]
        public string SName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("mname")]
        public string MName { get; set; }

        [JsonProperty("img")]
        public string Img { get; set; }
    }
}
