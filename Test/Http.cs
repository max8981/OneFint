using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    internal class Http
    {
        private static readonly HttpClient _client = new HttpClient();
        public static async Task LogNet()
        {
            var url = "http://192.168.210.200/ac_portal/login.php";
            _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
            //_client.DefaultRequestHeaders.AcceptLanguage.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("zh-CN"));
            //_client.DefaultRequestHeaders.AcceptLanguage.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("zh"));
            //_client.DefaultRequestHeaders.AcceptLanguage.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("en"));
            //_client.DefaultRequestHeaders.AcceptLanguage.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("en-GB"));
            //_client.DefaultRequestHeaders.AcceptLanguage.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("en-US"));
            //_client.DefaultRequestHeaders.AcceptCharset.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("UTF-8"));
            //_client.DefaultRequestHeaders.Add("User-Agent", "Safari/537.36 Edg/110.0.1587.41");
            var content = new StringContent("opr=pwdLogin&userName=yuwei&pwd=7c081e321d85&auth_tag=1678757300642&rememberPwd=0",Encoding.UTF8, "application/x-www-form-urlencoded");
            //content.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6");
            //content.Headers.Add("Connection", "keep-alive");
            //content.Headers.Add("Origin", "http://192.168.210.200");
            //content.Headers.Add("Referer", "http://192.168.210.200/ac_portal/20221016110356/pc.html?template=20221016110356&tabs=pwd&vlanid=0&_ID_=0&switch_url=&url=http://192.168.210.200/homepage/index.html&enable_cliauth=1&controller_type=&mac=d8-f8-83-81-ea-7f");
            //content.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36 Edg/110.0.1587.41"); 
            //content.Headers.ContentType=new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded; charset=UTF-8");
            var response =await _client.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseString);
            }
            else
            {
                Console.WriteLine(response.StatusCode.ToString());
            }
        }
    }
}
