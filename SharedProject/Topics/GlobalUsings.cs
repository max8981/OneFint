global using System.Text.Json.Serialization;
global using System.Text.Json;
global using System.Text.Unicode;

namespace SharedProject
{
    public class GlobalUsings
	{
        private static readonly JsonSerializerOptions options = new()
        {
            //PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            //Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All),
        };
        public static bool TryFromJson<T>(string json,out T? result)where T:new()
		{
            result = JsonSerializer.Deserialize<T>(json, options);
            return result!=null;
		}
        public static T? FromJson<T>(string json) where T : new()
        {
            var result = JsonSerializer.Deserialize<T>(json, options);
            return result;
        }
    }
}

