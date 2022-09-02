using System;
namespace SharedProject
{
	public class LayoutContent
	{
        [JsonPropertyName("pages")]
        public Page[] Pages { get; set; }
    }
}

