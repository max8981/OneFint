namespace VR文旅.Models.Requests
{
    internal class Standby : IRequest
    {
        [JsonIgnore]
        public string Url => "/client/standby";
    }
}
