using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace VR文旅
{
    internal static class Http
    {
        private static readonly HttpClient _client = new();
        public static async ValueTask<T> Post<T, O>(this O resquest, Dictionary<string, string>? query = null) 
            where T : Models.Responses.IResponse,new()
            where O : Models.Requests.IRequest,new()
        {
            var json = JsonSerializer.Serialize(resquest);
            var content = new StringContent(json);
            var querys=new List<string>();
            if (query != null)
                querys = query.Select(x => $"{x.Key}={x.Value}").ToList();
            var queryString = string.Join("&", querys);
            var url = $"{Global.HOST}{resquest.Url}?{queryString}";
            T result = new();
            try
            {
                var response = await _client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                    result = JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync())!;
                if (result == null)
                    Log.Default.Warn(typeof(T).FullName!, "JsonSerializer.Desrialize", json);
            }
            catch(Exception ex)
            {
                Log.Default.Error(ex, "Post");
            }
            return result!;
        }
    }
}
