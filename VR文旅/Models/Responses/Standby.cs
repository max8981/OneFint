namespace VR文旅.Models.Responses
{
    internal class Standby: IResponse
    {
        public Standby() { Standbies = Array.Empty<Standby>(); }
        [JsonPropertyName("total")]
        public int Total { get; set; }
        [JsonPropertyName("standbys")]
        public Standby[] Standbies { get; set; }
    }
}
