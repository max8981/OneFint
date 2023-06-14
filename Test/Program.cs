// See https://aka.ms/new-console-template for more information
using System;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Test;

var location = new Location("a", "a");
var a = location;
a.Province = "A";


//var tc = new TaskControl("\\OneFint");
//var a = tc.GetAllTasks();
//tc.CreateTask("test1",new Microsoft.Win32.TaskScheduler.ExecAction("shutdown.exe", "-r -f -t 0"),DateTime.Now.AddSeconds(5));
//tc.DeleteTask("test1");


var packs = new Downloader.DownloadPackage[]
{
    new Downloader.DownloadPackage()
    {
        Address="http://yuxtech.com:9000/videos/1e2c07c412bc22994d36c85e3c9ecbe4iyGR36.mp4",
        FileName="c:\\download\\专题3+支付结算.mp4",
    },
    new Downloader.DownloadPackage()
    {
        Address="http://yuxtech.com:9000/videos/cd5de0faee2db76744513288d97ff7a2QClu1R.mp4",
        FileName="c:\\download\\专题6+预防非法集资知识.mp4",
    },
    new Downloader.DownloadPackage()
    {
        Address="http://yuxtech.com:9000/videos/520656797d287de94007d89e6362b9fc9yV9tG.mp4",
        FileName="c:\\download\\专题4++理财产品知识.mp4",
    },
    new Downloader.DownloadPackage()
    {
        Address="http://yuxtech.com:9000/videos/39d7a758eed280ba1d0c280dc709fe5eEIgLyz.mp4",
        FileName="c:\\download\\LPR宣传片.mp4",
    },
};

var d=new DownloadController();
await d.Download(new Downloader.DownloadPackage() { Address = "http://yuxtech.com:9000/videos/39d7a758eed280ba1d0c280dc709fe5eEIgLyz.mp4" }, "c:\\download\\t.mp4");
//foreach (var pack in packs)
//    await d.Download(pack);
//d.Download("http://yuxtech.com:9000/videos/fb5647f9032129b35051b9a42c91a267iKM57b.mp4").Wait();
//SpinWait.SpinUntil(() => {
//    var task = d[pack.FileName];
//    //Console.CursorTop = task.Id;
//    //Console.CursorLeft = 0;
//    //Console.WriteLine($"{task.Id} {task.FileName} {task.FileSize} {task.DownloadSpeed}");
//    return task.IsComplete;
//});

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
