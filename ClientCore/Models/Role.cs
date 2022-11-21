namespace ClientCore.Models
{
    /// <summary>
    /// 角色
    /// </summary>
    public class Role : BaseDateAt
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }
        /// <summary>
        /// 角色name
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("creator")]
        public object? Creator { get; set; }
    }
}

