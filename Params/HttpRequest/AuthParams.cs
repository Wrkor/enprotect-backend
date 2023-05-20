using Newtonsoft.Json;

namespace webapi.Params.HttpRequest
{
    public class AuthParams
    {
        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
