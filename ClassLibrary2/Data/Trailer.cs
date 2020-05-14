using Newtonsoft.Json;
using System.Collections.Generic;

namespace MovieBuffLib
{
    public partial class Trailer
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("results")]
        public List<Result> Results { get; set; }

        public static Trailer FromJson(string json) => JsonConvert.DeserializeObject<Trailer>(json, Converter.Settings);
    }

    public partial class Result
    {
        [JsonProperty("iso_3166_1")]
        public string Iso31661 { get; set; }

        [JsonProperty("iso_639_1")]
        public string Iso6391 { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("site")]
        public string Site { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}