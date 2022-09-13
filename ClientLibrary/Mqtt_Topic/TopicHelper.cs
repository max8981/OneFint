using System;
using System.Text.Json;
using System.Text.Unicode;

namespace SharedProject
{
	public static class TopicHelper
	{
        private static readonly JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All),
        };
        public static string ToJson<T>(T type)
		{
			var json = JsonSerializer.Serialize(type,options);
			return json;
		}
	}
}

