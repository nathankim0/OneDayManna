using System.Collections.Generic;
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

    public class JsonMannaOtherLanguageModel
    {
        [JsonProperty("verses")]
        public List<ContentVerse> Verses { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }
    }

    public class ContentVerse
    {
        [JsonProperty("book_id")]
        public string BookId { get; set; }

        [JsonProperty("book_name")]
        public string BookName { get; set; }

        [JsonProperty("chapter")]
        public int Chapter { get; set; }

        [JsonProperty("verse")]
        public int Verse { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
