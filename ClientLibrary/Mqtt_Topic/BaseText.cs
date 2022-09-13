using System;
namespace ClientLibrary
{
	public class BaseText
	{
        /// <summary>
        /// 文本，或网址
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; }
        /// <summary>
        /// 背景色
        /// </summary>
        [JsonPropertyName("background_color")]
        public string BackgroundColor { get; set; }

        /// <summary>
        /// 字体颜色
        /// </summary>
        [JsonPropertyName("font_color")]
        public string FontColor { get; set; }
        /// <summary>
        /// 字体大小
        /// </summary>
        [JsonPropertyName("font_size")]
        public int? FontSize { get; set; }

        /// <summary>
        /// 文本水平位置，可选0,1,2,3; 0：UNKNOWN_CONTENT_TEXT_HORIZONTAL_POSITION；1: LEFT; 2: CENTER; 3: RIGHT;
        /// </summary>
        [JsonPropertyName("horizontal")]
        public HorizontalEnum Horizontal { get; set; }
        /// <summary>
        /// 文本垂直位置, 可选0,1,2,3; 0: UNKNOWN_CONTENT_TEXT_VERTICAL_POSITION; 1: TOP; 2: CENTER; 3:
        /// BOTTOM;
        /// </summary>
        [JsonPropertyName("vertical")]
        public VerticalEnum Vertical { get; set; }
        public enum HorizontalEnum
        {
            UNKNOWN_CONTENT_TEXT_HORIZONTAL_POSITION,
            LEFT,
            CENTER,
            RIGHT,
        }
        public enum VerticalEnum
        {
            UNKNOWN_CONTENT_TEXT_VERTICAL_POSITION,
            TOP,
            CENTER,
            BOTTOM,
        }
    }
}

