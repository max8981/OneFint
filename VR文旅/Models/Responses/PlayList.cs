namespace VR文旅.Models.Responses
{
    internal class PlayList: IResponse
    {
        public PlayList() { Scenarios = Array.Empty<Scenario>(); }
        [JsonPropertyName("total")]
        public int Total { get; set; }
        [JsonPropertyName("scenarios")]
        public Scenario[] Scenarios { get; set; }
    }
}
