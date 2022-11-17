using ClientLibrary.ClientToServer;
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
        void ShutDown();
        void Reboot();
        void ScreenActivation(bool activation);
        void SetDate(DateTime date);
        Action<int> SetVolume { get; }
        Action<int> ScreenPowerOff { get; }
        Action<int> ScreenPowerOn { get; }
        Action<int> ScreenBrightness { get; }
        Action DeleteTempFiles { get; }
        Action<string[]> DeleteFiles { get; }
        int Volume { get; }
        void SaveConfiguration(string name,string value);
        string LoadConfiguration(string name);
        Action<string, string> WriteLog { get;}
        UIs.IPageController[] PageControllers { get; set; }
        void ShowMessage(string message, TimeSpan delay);
        ClientConfig Config { get; set; }
    }
}
