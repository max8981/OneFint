using System;
namespace ClientLibrary
{
	public class LayoutContent
	{
        [JsonPropertyName("pages")]
        public Page[] Pages { get; set; }
    }
}

