using Newtonsoft.Json;
using webapi.Entities;

namespace webapi.Params.HttpResponse
{
    public class OffersSendParams
    {
        [JsonProperty("offerid")]
        public int OfferId { get; set; }

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

        public OffersSendParams(Offer offer)
        {
            OfferId = offer.OfferId;

            Title = offer.Title;
            Description = offer.Description;
            Imgs = offer.Imgs;
            Status = offer.Status;
            Date = offer.Date;
        }
    }
}
