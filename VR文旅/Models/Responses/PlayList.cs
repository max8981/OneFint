namespace VR文旅.Models.Responses
{
    internal class PlayList
    {
        public PlayList()
        {
            Scenarios = Array.Empty<Scenario>();
            Devices = Array.Empty<Device>();
        }
        [JsonPropertyName("total")]
        public int Total { get; set; }
        [JsonPropertyName("device")]
        public Device[] Devices { get; set; }
        [JsonPropertyName("scenarios")]
        public Scenario[] Scenarios { get; set; }
    }
}
