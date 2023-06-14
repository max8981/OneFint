using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static VR文旅.Models.Standbys;

namespace VR文旅.Models
{
    internal struct Scenario
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("scenario_name")]
        public string? ScenarioName { get; set; }
        [JsonPropertyName("province")]
        public string? Province { get; set; }
        [JsonPropertyName("city")]
        public string? City { get; set; }
        [JsonPropertyName("scenario_category")]
        public string? ScenarioCategory { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("web_link")]
        public string? WebLink { get; set; }
        [JsonPropertyName("thumb_url")]
        public string? ThumbUrl { get; set; }
        public async Task<BitmapImage> GetBitmapAsync()
        {
            var filename = new Uri(ThumbUrl??"").Segments.Last();
            var ext = System.IO.Path.GetExtension(filename);
            var file = new FileInfo($"./Cache/{Id}{ScenarioName}{ext}");
            if (await file.DownloadAsync(ThumbUrl))
                return Global.GetBitmap(file);
            else
                return Global.GetBitmap("Logo");
        }
    }
}
