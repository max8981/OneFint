namespace ClientLibrary.Models
{
    public class LayoutContent
    {
        [JsonPropertyName("pages")]
        public Page[]? Pages { get; set; }
    }
}

