using Newtonsoft.Json;

namespace webapi.Entities
{
    public class User
    {
        [JsonProperty("userid")]
        public Guid UserId { get; set; }

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
        public string Img { get; set; }

        [JsonProperty("push_offense_sms")]
        public bool Push_Offense_SMS { get; set; }

        [JsonProperty("push_offense_email")]
        public bool Push_Offense_Email { get; set; }

        [JsonProperty("push_offer_email")]
        public bool Push_Offer_Email { get; set; }
    }
}
