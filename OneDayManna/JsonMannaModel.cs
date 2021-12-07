using Newtonsoft.Json;

namespace OneDayManna
{
    public class JsonMannaModel
    {
        [JsonProperty("verse")]
        public string Verse { get; set; } = "";

        [JsonProperty("contents")]
        public string[] Contents { get; set; } = { "", "" };
    }
}
