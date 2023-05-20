using Newtonsoft.Json;
using webapi.Entities;

namespace webapi.Params.HttpResponse
{
    public class OffenseSendParams
    {
        [JsonProperty("offenseid")]
        public int OffenseId { get; set; }

        [JsonProperty("snameEmployee")]
        public string? SNameEmployee { get; set; }

        [JsonProperty("nameEmployee")]
        public string? NameEmployee { get; set; }

        [JsonProperty("mnameEmployee")]
        public string? MNameEmployee { get; set; }

        [JsonProperty("imgEmployee")]
        public string? ImgEmployee { get; set; }

        [JsonProperty("snameExpert")]
        public string SNameExpert { get; set; }

        [JsonProperty("nameExpert")]
        public string NameExpert { get; set; }

        [JsonProperty("mnameExpert")]
        public string MNameExpert { get; set; }

        [JsonProperty("imgExpert")]
        public string ImgExpert { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("imgs")]
        public string? Imgs { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        public OffenseSendParams(Offense offense, User expert, User? employee = null)
        {
            OffenseId = offense.OffenseId;

            SNameExpert = expert.SName;
            MNameExpert = expert.MName;
            NameExpert = expert.Name;
            ImgExpert = expert.Img;

            if (employee != null)
            {
                SNameEmployee = employee.SName;
                MNameEmployee = employee.MName;
                NameEmployee = employee.Name;
                ImgEmployee = employee.Img;
            }

            Title = offense.Title;
            Description = offense.Description;
            Category = offense.Category;
            Imgs = offense.Imgs;
            Status = offense.Status;
            Date = offense.Date;
        }
    }
}
