using System;
namespace SharedProject
{
    /// <summary>
    /// 角色
    /// </summary>
    public class Role
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
        public string Name { get; set; }
        [JsonPropertyName("creator")]
        public object Creator { get; set; }
        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public string UpdatedAt { get; set; }
    }
}

