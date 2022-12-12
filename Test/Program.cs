// See https://aka.ms/new-console-template for more information
using System;
using System.Text;
using System.Text.Json;

var i = 0.GetType();
var s = "0".GetType();

Console.WriteLine(i.Equals(typeof(System.Int32)));
Console.WriteLine(i.Equals(typeof(System.String)));

var url = "http://47.101.178.160:9009/videos/ebb403229cb089de04c40b7ddc81ba60CF17QQ.mp4";
var m3u8 = "http://112.50.243.8/PLTV/88888888/224/3221225922/1.m3u8";
//var filename = "ceshi.mp4";

var uri=new Uri(m3u8);

Dictionary<string, bool> _disengages = new();


var a =_disengages["0"];


while (true)
{
    Console.ReadKey();
};
