namespace VR文旅.Models.Requests
{
    internal interface IRequest
    {
        [JsonIgnore]
        string Url { get; }
    }
}
