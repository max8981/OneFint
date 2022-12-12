namespace VR文旅.Models.Responses
{
    internal class Access: IResponse
    {
        [JsonPropertyName("access-token")]
        public string? AccessToken { get; set; }
        [JsonPropertyName("expired_at")]
        public string? ExpiredAt { get; set; }
    }
}
