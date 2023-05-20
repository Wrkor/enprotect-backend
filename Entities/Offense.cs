using Newtonsoft.Json;

namespace webapi.Entities
{
    public class Offense
    {
        [JsonProperty("offenseid")]
        public int OffenseId { get; set; }

        [JsonProperty("employeeid")]
        public Guid? EmployeeId { get; set; }

        [JsonProperty("expertid")]
        public Guid ExpertId { get; set; }

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
    }
}
