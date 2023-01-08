using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VR文旅.Models
{
    internal class PLayLists : Http.IResponse
    {
        private const string PATH = "/api/client/playlists/";
        private static PlayList[] _playLists = Array.Empty<PlayList>();
        public static event Action<PLayLists>? PlayListChanged;
        [JsonPropertyName("playlists")]
        public PlayList[] PlayLists { get => _playLists; set => _playLists = value; }
        public bool Success { get; set; }
        public static async Task<bool> GetPLayList(Location[] locations, string[] categorys)
        {
            var request = new
            {
                mac_address = "a",
                locations,
                scenario_category = "",
            };
            var response = await request.PostAsync<PLayLists>(PATH);
            if (response.Success)
                PlayListChanged?.Invoke(response);
            return response.Success;
        }
        public static Scenario[] GetScenarios(int page, int size)
        {
            return _playLists.ToArray().Skip(page * size).Take(size).Select(x=>x.Scenario).ToArray();
        }
        public static int Count => _playLists.Length;
        internal class PlayList
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }
            [JsonPropertyName("device")]
            public Device Device { get; set; }
            [JsonPropertyName("scenario")]
            public Scenario Scenario { get; set; }
        }
    }
}
