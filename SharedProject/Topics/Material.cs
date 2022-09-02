using System;
namespace SharedProject
{
    /// <summary>
    /// 素材
    /// </summary>
    public class Material:BaseDateAt
    {
        /// <summary>
        /// 素材ID
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }
        /// <summary>
        /// 素材name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
        /// <summary>
        /// 枚举：0,1,2,3  枚举备注:  0 :UNKNOWN_MATERIAL_TYPE  1:MATERIAL_TYPE_AUDIO
        /// 2:MATERIAL_TYPE_VIDEO  3:MATERIAL_TYPE_IMAGE
        /// </summary>
        [JsonPropertyName("material_type")]
        public MaterialTypeEnum MaterialType { get; set; }
        [JsonPropertyName("folder")]
        public Folder Folder { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        [JsonPropertyName("content")]
        public string Content { get; set; }

        public enum MaterialTypeEnum
        {
            UNKNOWN_MATERIAL_TYPE,
            MATERIAL_TYPE_AUDIO,
            MATERIAL_TYPE_VIDEO,
            MATERIAL_TYPE_IMAGE,
            MATERIAL_TYPE_TEXT=5,
        }
    }
}

