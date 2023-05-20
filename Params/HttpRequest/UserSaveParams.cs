using Newtonsoft.Json;

namespace webapi.Params.HttpRequest
{
    public class UserSaveParams
    {
        [JsonProperty("push_offense_sms")]
        public bool Push_Offense_SMS { get; set; }

        [JsonProperty("push_offense_email")]
        public bool Push_Offense_Email { get; set; }

        [JsonProperty("push_offer_email")]
        public bool Push_Offer_Email { get; set; }
    }
}
