using Newtonsoft.Json;
using webapi.Entities;

namespace webapi.Params.HttpResponse
{
    public class UserDataParams
    {
        [JsonProperty("sname")]
        public string SName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("mname")]
        public string MName { get; set; }

        [JsonProperty("roleid")]
        public int RoleId { get; set; }

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

        public UserDataParams(User user)
        {
            SName = user.SName;
            Name = user.Name;
            MName = user.MName;

            Job = user.Job;
            Img = user.Img;
            RoleId = user.RoleId;

            Push_Offense_SMS = user.Push_Offense_SMS;
            Push_Offense_Email = user.Push_Offense_Email;
            Push_Offer_Email = user.Push_Offer_Email;
        }
    }
}
