using Newtonsoft.Json;
using webapi.Entities;

namespace webapi.Params.HttpResponse
{
    public class OffensesSendParams
    {
        [JsonProperty("offenseid")]
        public int OffenseId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        public OffensesSendParams(Offense offense)
        {
            OffenseId = offense.OffenseId;

            Title = offense.Title;
            Description = offense.Description;
            Status = offense.Status;
            Date = offense.Date;
        }
    }
}
