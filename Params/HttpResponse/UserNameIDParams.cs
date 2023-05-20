using Newtonsoft.Json;
using webapi.Entities;

namespace webapi.Params.HttpResponse
{
    public class UserNameIDParams
    {
        [JsonProperty("sname")]
        public string SName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("mname")]
        public string MName { get; set; }

        [JsonProperty("userid")]
        public Guid UserId { get; set; }

        public UserNameIDParams(User user)
        {
            SName = user.SName;
            Name = user.Name;
            MName = user.MName;
            UserId = user.UserId;
        }
    }
}
