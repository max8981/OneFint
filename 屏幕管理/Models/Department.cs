namespace 屏幕管理.Models
{
    /// <summary>
    /// 部门
    /// </summary>
    public class Department : BaseDateAt
    {
        /// <summary>
        /// 部门ID
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }
        /// <summary>
        /// 部门name
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("creator")]
        public object? Creator { get; set; }
    }
}

