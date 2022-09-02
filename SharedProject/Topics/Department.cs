using System;
namespace SharedProject
{
    /// <summary>
    /// 部门
    /// </summary>
    public class Department
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
        public string Name { get; set; }
        [JsonPropertyName("creator")]
        public object Creator { get; set; }
        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public string UpdatedAt { get; set; }
    }
}

