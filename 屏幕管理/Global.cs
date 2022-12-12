global using System;
global using System.Text.Json;
global using System.Text.Json.Serialization;



namespace 屏幕管理
{
    internal class Global
    {
        internal static Action<string, int> ShowMessage => (c, d) => { };
        internal static Action<string, string> MQTTLog => (t, c) => { };
    }
}
