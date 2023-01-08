// See https://aka.ms/new-console-template for more information
using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Test;


//var url = "http://47.101.178.160:9009/videos/ebb403229cb089de04c40b7ddc81ba60CF17QQ.mp4";
//var m3u8 = "http://112.50.243.8/PLTV/88888888/224/3221225922/1.m3u8";
//var jpg = "http://minio.mw.zqdev.project.virtueit.net/images/8e94f945afdfb9bb946edc25cfa463c9fUmzev.jpg";

//var d = new DownloadController.DownloadTask(0, jpg, "jpg");
//d.Start();
//SpinWait.SpinUntil(() => { Console.WriteLine(d.DownloadSpeed);return d.IsComplete; } );
//var filename = "ceshi.mp4";

//var uri=new Uri(m3u8);
Dictionary<Location, bool> dic = new();
var a = new Location("a", "b");
var b = new Location("a", "b");
Console.WriteLine(a.Equals(b));
if (dic.TryAdd(a, false))
    dic[b] = true;


while (true)
{
    Console.ReadKey();
};


internal struct Location
{
    public Location(string province, string city)
    {
        Province = province;
        City = city;
    }
    [JsonPropertyName("province")]
    public string Province { get; set; }
    [JsonPropertyName("city")]
    public string City { get; set; }
}