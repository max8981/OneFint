using CefSharp.DevTools.Debugger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VR文旅.Models
{
    internal class ScenarioCategories:Http.IResponse
    {
        private const string PATH = "/api/client/scenario_categories/";
        private static string[] _categories = Array.Empty<string>();
        private static readonly Dictionary<string, bool> _types = new();
        [JsonPropertyName("categories")]
        public string[] Categories { get => _categories; set => _categories = value; }
        public bool Success { get; set; }

        public static async Task<bool> GetScenarioCategoriesAsync()
        {
            var request = new
            {
                org_id = Systems.Config.OrdId,
            };
            var response = await request.PostAsync<ScenarioCategories>(PATH);
            if (response.Success)
                _categories = response.Categories;
            foreach (var item in _categories)
                _ = _types.TryAdd(item, false);
            return response.Success;
        }
        public static string[] GetScenarioCategories() => _categories;
        public static Dictionary<string,bool> Types => _types;
        public static void SetValue(string key,bool value)
        {
            if (!_types.TryAdd(key, value))
                _types[key] = value;
        }
        public static bool GetValue(string key)
        {
            _types.TryGetValue(key, out bool result);
            return result;
        }
        public static string[] GetTypes() => _types.Where(x => x.Value).Select(x => x.Key).ToArray();
    }
}
