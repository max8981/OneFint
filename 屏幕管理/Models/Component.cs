namespace 屏幕管理.Models
{
    public class Component
    {
        /// <summary>
        /// 组件ID
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }
        /// <summary>
        /// 组件UUID
        /// </summary>
        [JsonPropertyName("uuid")]
        public string? Uuid { get; set; }
        /// <summary>
        /// 组件name
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        /// <summary>
        /// 左边距
        /// </summary>
        [JsonPropertyName("x")]
        public int X { get; set; }

        /// <summary>
        /// 上边距
        /// </summary>
        [JsonPropertyName("y")]
        public int Y { get; set; }
        /// <summary>
        /// 组件宽度
        /// </summary>
        [JsonPropertyName("w")]
        public int W { get; set; }
        /// <summary>
        /// 组件高度
        /// </summary>
        [JsonPropertyName("h")]
        public int H { get; set; }
        /// <summary>
        /// 置于第几层
        /// </summary>
        [JsonPropertyName("z")]
        public int Z { get; set; }
        /// <summary>
        /// 可选0,1,2,3,4,5,6，若component_type为网址控件，则网址写到text里; 0:UNKNOWN_COMPONENT_TYPE; 1:IMAGE;
        /// 2:BROWSER; 3:TEXT; 4:VIDEO; 5: CLOCK; 6:EXHIBITION_STAND
        /// </summary>
        [JsonPropertyName("component_type")]
        public Enums.ComponentTypeEnum ComponentType { get; set; }
        /// <summary>
        /// 可选0,1,2,3；0: UNKNOWN_COMPONENT_INTERCHANGE_EFFECT; 1: NULL; 2:FADE_IN_AND_OUT;
        /// 3:PUSH_IN_AND_OUT
        /// </summary>
        [JsonPropertyName("interchange_effect")]
        public Enums.InterchangeEffectEnum InterchangeEffect { get; set; }
        [JsonPropertyName("text")]
        public BaseText? Text { get; set; }
        /// <summary>
        /// 时钟类型，可选0，1，2；0: UNKNOWN_CLOCK_TYPE; 1: TYPE_1; 2: TYPE_2
        /// </summary>
        [JsonPropertyName("clock_type")]
        public Enums.ClockTypeEnum ClockType { get; set; }
        /// <summary>
        /// 字体颜色
        /// </summary>
        [JsonPropertyName("font_color")]
        public string? FontColor { get; set; }
        /// <summary>
        /// 背景色
        /// </summary>
        [JsonPropertyName("background_color")]
        public string? BackgroundColor { get; set; }
        /// <summary>
        /// 字体透明度
        /// </summary>
        [JsonPropertyName("transparency")]
        public int Transparency { get; set; }
        /// <summary>
        /// 垫片
        /// </summary>
        [JsonPropertyName("pad_url")]
        public string? PadUrl { get; set; }
    }
}

