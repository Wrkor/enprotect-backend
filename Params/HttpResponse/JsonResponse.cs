using Newtonsoft.Json;

namespace webapi.Params.HttpResponse
{
    public class JsonResponse
    {
        [JsonProperty("result")]
        public bool Result { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        public JsonResponse(bool result, string message, string data)
        {
            Result = result;
            Message = message;
            Data = data;
        }
    }
}
