using Newtonsoft.Json;

namespace webapi.Params.HttpRequest
{
    public class OffenseParams
    {
        [JsonProperty("employeeid")]
        public Guid? EmployeeId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("imgs")]
        public IFormFileCollection? Imgs { get; set; }

    }
}
