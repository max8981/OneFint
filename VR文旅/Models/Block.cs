using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VR文旅.Models
{
    internal class Block
    {
        [JsonPropertyName("error")]
        public int Error { get; set; }
        [JsonPropertyName("blocks")]
        public string[] Blocks { get; set; } = Array.Empty<string>();
        private const string PATH = "/api/client/blocks/";
        public static async Task<string[]> GetBlocks()
        {
            var responseJson = await Http.GetAsync(Systems.Config.Server + PATH);
            Block? block = null;
            if (!string.IsNullOrEmpty(responseJson))
                block = JsonSerializer.Deserialize<Block>(responseJson);
            if (block?.Error == 0)
                return block.Blocks;
            else
                return Array.Empty<string>();
        }
    }
}
