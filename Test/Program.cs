// See https://aka.ms/new-console-template for more information
using ClientLibrary;
using MQTTnet;
using MQTTnet.Client;
using System;
using System.Text;
using System.Text.Json;




//var url = "http://47.101.178.160:9009/videos/ebb403229cb089de04c40b7ddc81ba60CF17QQ.mp4";
//var filename = "ceshi.mp4";

var json = System.IO.File.ReadAllText("d:/json.txt");
var result = JsonSerializer.Deserialize<ClientLibrary.ServerToClient.BaseContent>(json);





while (true)
{
    Console.ReadKey();
};
