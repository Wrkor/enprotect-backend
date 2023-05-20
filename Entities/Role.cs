using Newtonsoft.Json;

namespace webapi.Entities
{
    public class Role
    {
        [JsonProperty("roleid")]
        public int RoleId { get; set; }

        [JsonProperty("role_name")]
        public string Role_Name { get; set; }

        [JsonProperty("access")]
        public string Access { get; set; }
    }
}
