using Newtonsoft.Json;
using webapi.Entities;

namespace webapi.Params.HttpResponse
{
    public class OfferSendParams
    {
        [JsonProperty("offerid")]
        public int OfferId { get; set; }

        [JsonProperty("snameEmployee")]
        public string SNameEmployee { get; set; }

        [JsonProperty("nameEmployee")]
        public string NameEmployee { get; set; }

        [JsonProperty("mnameEmployee")]
        public string MNameEmployee { get; set; }

        [JsonProperty("imgEmployee")]
        public string ImgEmployee { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("imgs")]
        public string? Imgs { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        public OfferSendParams(Offer offer, User employee)
        {
            OfferId = offer.OfferId;

            SNameEmployee = employee.SName;
            NameEmployee = employee.Name;
            MNameEmployee = employee.MName;
            ImgEmployee = employee.Img;

            Title = offer.Title;
            Description = offer.Description;
            Imgs = offer.Imgs;
            Status = offer.Status;
            Date = offer.Date;
        }
    }
}
