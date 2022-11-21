namespace ClientCore.Models
{
    public class Owner : BaseDateAt
    {
        /// <summary>
        /// 负责人ID
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }
        /// <summary>
        /// 唯一账号
        /// </summary>
        [JsonPropertyName("account")]
        public string? Account { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        [JsonPropertyName("username")]
        public string? Username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [JsonPropertyName("password")]
        public string? Password { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        [JsonPropertyName("phone")]
        public string? Phone { get; set; }
        [JsonPropertyName("role")]
        public Role? Role { get; set; }
        [JsonPropertyName("department")]
        public Department? Department { get; set; }
        /// <summary>
        /// 是否初次登录
        /// </summary>
        [JsonPropertyName("first_login")]
        public bool FirstLogin { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        [JsonPropertyName("enable")]
        public bool Enable { get; set; }
    }
}

