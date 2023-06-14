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
        private static PlayList[] _playLists = new PlayList[]
        {
            new PlayList
            {
                Scenario=new Scenario
                {
                    City="测试城市",
                    Description="测试说明",
                    Province="测试省份",
                    ScenarioCategory="测试分类",
                    ScenarioName="测试名字",
                    WebLink="https://www.720yun.com/vr/70c26afkyya",
                    ThumbUrl="https://tse2-mm.cn.bing.net/th/id/OIP-C.XkSiVLVjDG9PGVOK_aZGnwAAAA?pid=ImgDet&rs=1",
                    Id=0,
                }
            },
            new PlayList{ Scenario=new Scenario
            {
                    City="城市",
                    Description="说明",
                    Province="省份",
                    ScenarioCategory="分类",
                    ScenarioName="江南水乡",
                    WebLink ="https://www.720yun.com/vr/8fc2cu869ba",
                    ThumbUrl="https://tse2-mm.cn.bing.net/th/id/OIP-C.XkSiVLVjDG9PGVOK_aZGnwAAAA?pid=ImgDet&rs=1",
                    Id=1,
            }
            }
        };
        public static event Action<PLayLists>? PlayListChanged;
        [JsonPropertyName("playlists")]
        public PlayList[] PlayLists { get; set; } = Array.Empty<PlayList>();
        public bool Success { get; set; }
        public static async Task<bool> GetPLayList(Location[] locations, string categorys)
        {
            var request = new
            {
                mac_address = Systems.Config.MacAddress,
                locations,
                scenario_category = categorys,
            };
            var response = await request.PostAsync<PLayLists>(PATH);
            if (response.Success)
            {
                _playLists = response.PlayLists;
                PlayListChanged?.Invoke(response);
            }
            return response.Success;
        }
        public static Scenario[] GetScenarios(int page, int size)
        {
            page = Math.Clamp(page, 0, _playLists.Length);
            size = Math.Clamp(size, 1, int.MaxValue);
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
