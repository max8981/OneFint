using System;
using System.Collections.Generic;
using System.Text;

namespace ClientCore
{
    internal interface IClient
    {
        void Connect();
        void Send(string topic,string jsonb);
        void ShutDown();
        void Hibernate();
        void Reboot();
        void ScreenActivation(bool activation);
        void SetDate(DateTime date);
        void SetVolume(int value);
        int GetVolume();
        void DeleteFiles(string[] files);
        void ShowMessage(string message, TimeSpan delay);
    }
}
