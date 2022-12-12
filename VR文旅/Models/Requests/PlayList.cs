namespace VR文旅.Models.Requests
{
    internal class PlayList: IRequest
    {
        public PlayList() { }
        [JsonPropertyName("page_index")]
        public int PageIndex { get; set; }
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }
        [JsonPropertyName("province")]
        public string? Province { get; set; }
        [JsonPropertyName("city")]
        public string? City { get; set; }
        [JsonPropertyName("category")]
        public string? Category { get; set; }
        [JsonIgnore]
        public string Url => "/client/playlist";
    }
}
