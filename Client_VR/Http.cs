using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Client_VR
{
    internal class Http
    {
        private readonly static HttpClient _httpClient = new HttpClient();
        public static async void OptionsAsync(string url)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Options, url);
            var response = await _httpClient.SendAsync(httpRequest);
            if (response.IsSuccessStatusCode)
            {
                var setCookies = response.Headers.Where(_ => _.Key == "set-cookie").ToArray();
                foreach (var item in setCookies)
                {
                    _httpClient.DefaultRequestHeaders.Add("Cookie", item.Value);
                }
            }
        }
        public static async Task<string> GetAsync(string url)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Get, url);
            httpRequest.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            httpRequest.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/plain"));
            httpRequest.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
            httpRequest.Headers.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));
            httpRequest.Headers.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("deflate"));
            httpRequest.Headers.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("br"));
            httpRequest.Headers.AcceptLanguage.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("zh-CN"));
            httpRequest.Headers.AcceptLanguage.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("zh"));
            httpRequest.Headers.Add("App-Key", "eByjUyLDG2KtkdhuTsw2pY46Q3ceBPdT");
            httpRequest.Headers.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true,
            };
            httpRequest.Headers.ConnectionClose = false;
            httpRequest.Headers.Add("Cookie", "Hm_lvt_08a05dadf3e5b6d1c99fc4d862897e31=1666594387; 720yun_v8_session=Z3gp1lBx5JV8DXRmyEWwMQqbWxZPY6rDCNOeRxzdAnvNOAbKez70Y296jordLP4G; xn_dvid_kf_20097=B95FEA-E14DFD75-1532-8147-4383-08C436B8BB78; Hm_lpvt_08a05dadf3e5b6d1c99fc4d862897e31=1666664985; xn_sid_kf_20097=1666594387961126");
            httpRequest.Headers.Host = "api-oms.720yun.com";
            httpRequest.Headers.Add("Origin", "api-oms.720yun.com");
            httpRequest.Headers.Add("Pragma", "no-cache");
            httpRequest.Headers.Referrer = new Uri("https://www.720yun.com/");
            httpRequest.Headers.Add("Sec-Fetch-Dest", "empty");
            httpRequest.Headers.Add("Sec-Fetch-Mode", "no-cors");
            httpRequest.Headers.Add("Sec-Fetch-Site", "same-site");
            httpRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:105.0) Gecko/20100101 Firefox/105.0");
            
            var response = await _httpClient.SendAsync(httpRequest);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return response.StatusCode.ToString();
            }
        }
    }
}
