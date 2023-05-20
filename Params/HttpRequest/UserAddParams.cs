using Newtonsoft.Json;

namespace webapi.Params.HttpRequest
{
    public class UserAddParams
    {
        [JsonProperty("expertid")]
        public Guid? ExpertId { get; set; }

        [JsonProperty("roleid")]
        public int RoleId { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("sname")]
        public string SName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("mname")]
        public string MName { get; set; }

        [JsonProperty("job")]
        public string Job { get; set; }

        [JsonProperty("img")]
        public IFormFile? Img { get; set; }
    }
}
