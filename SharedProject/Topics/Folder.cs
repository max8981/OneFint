using System;
namespace SharedProject
{
    public partial class Folder:BaseDateAt
    {
        /// <summary>
        /// 目录id
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// 目录名称
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// 父ID
        /// </summary>
        [JsonPropertyName("parent_id")]
        public int ParentId { get; set; }
    }
}

