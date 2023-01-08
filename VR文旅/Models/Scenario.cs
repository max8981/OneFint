namespace VR文旅.Models
{
    internal struct Scenario
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("scenario_name")]
        public string? ScenarioName { get; set; }
        [JsonPropertyName("province")]
        public string? Province { get; set; }
        [JsonPropertyName("city")]
        public string? City { get; set; }
        [JsonPropertyName("scenario_category")]
        public string? ScenarioCategory { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("web_link")]
        public string? WebLink { get; set; }
        [JsonPropertyName("thumb_url")]
        public string? ThumbUrl { get; set; }
    }
}
