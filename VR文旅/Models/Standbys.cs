using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VR文旅.Models
{
    internal class Standbys:Http.IResponse
    {
        private const string PATH = "/api/client/standbys/";
        private static Standby[] _standbies = Array.Empty<Standby>();
        [JsonPropertyName("total")]
        public int Total { get; set; }
        [JsonPropertyName("standbys")]
        public Standby[] Standbies { get => _standbies; set => _standbies = value; }
        public bool Success { get; set; }

        public static async Task<Standby[]> GetStandbiesAsync()
        {
            var request = new
            {
                org_id = Systems.Config.OrdId,
            };
            var response = await request.PostAsync<Standbys>(PATH);
            if (response.Success)
                return response.Standbies;
            else
                return Array.Empty<Standby>();
        }
        internal struct Standby
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }
            [JsonPropertyName("org")]
            public Org Org { get; set; }
            [JsonPropertyName("standby_name")]
            public string? Name { get; set; }
            [JsonPropertyName("resource_url")]
            public string? ResourceUrl { get; set; }
            [JsonPropertyName("resource_type")]
            public ResourceTypeEnum ResourceType { get; set; }
            [JsonPropertyName("mute")]
            public bool Mute { get; set; }
            [JsonPropertyName("duration")]
            public int Duration { get; set; }
            [JsonPropertyName("valid_start_time")]
            public string? StartedAt { set => _ = DateTime.TryParse(value, out Start); }
            [JsonPropertyName("valid_end_time")]
            public string? EndedAt { set => _ = DateTime.TryParse(value, out End); }
            [JsonPropertyName("created_by")]
            public string? Created_by { get; set; }
            [JsonPropertyName("status")]
            public int Status { get; set; }
            [JsonPropertyName("created_at")]
            public string? Created_at { get; set; }
            [JsonPropertyName("updated_at")]
            public string? Updated_at { get; set; }
            public DateTime Start;
            public DateTime End;
        }
        internal enum ResourceTypeEnum
        {
            Image=1,
            Video=2,
        }
    }
}
