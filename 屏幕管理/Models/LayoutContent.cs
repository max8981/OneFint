namespace 屏幕管理.Models
{
    public class LayoutContent
    {
        [JsonPropertyName("pages")]
        public Page[]? Pages { get; set; }
    }
}

