using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class ClientController : ClientCore.IClientController
    {
        public Action<string, string> Receive { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public void DeleteFiles(string[] files)
        {
            throw new NotImplementedException();
        }

        public int GetVolume()
        {
            throw new NotImplementedException();
        }

        public void Hibernate()
        {
            throw new NotImplementedException();
        }

        public void Reboot()
        {
            throw new NotImplementedException();
        }

        public void ScreenActivation(bool activation)
        {
            throw new NotImplementedException();
        }

        public void Send(string topic, string json)
        {
            throw new NotImplementedException();
        }

        public void SetDate(DateTime date)
        {
            throw new NotImplementedException();
        }

        public void SetVolume(int value)
        {
            throw new NotImplementedException();
        }

        public void ShowMessage(string message, TimeSpan delay)
        {
            throw new NotImplementedException();
        }

        public void ShutDown()
        {
            throw new NotImplementedException();
        }
    }
}
