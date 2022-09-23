using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary
{
    public interface IClient
    {
        Action<DateTime> PowerOn { get; }
        Action<DateTime> PowerOff { get; }
        Action Reboot { get; }
        Action<int> SetVolume { get; }
        Action<int> ScreenPowerOff { get; }
        Action<int> ScreenPowerOn { get; }
        Action<int> ScreenBrightness { get; }
        Action DeleteTempFiles { get; }
        int Volume { get; }
        void Save<T>(string key,T value);
        T Load<T>(string key)where T:new();
        Action<string, string> WriteLog { get;}
        UIs.IPageController[] PageControllers { get; set; }
        ClientConfig Config { get; set; }
    }
}
