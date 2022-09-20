namespace ClientLibrary.Models
{
    /// <summary>
    /// 硬件厂商
    /// </summary>
    public class Hardware : BaseDateAt
    {
        /// <summary>
        /// 硬件厂商ID
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }
        /// <summary>
        /// 硬件厂商名称
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}

