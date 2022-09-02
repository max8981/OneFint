using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SharedProject
{
    internal interface IClientControl
    {
        public void Conntect();
        public void Subscribe();
        public void HeartBeat();
        public void Send(string topic, string json);
        public void Receive(TopicEnum topic,string json);
        public void Close();
        public enum TopicEnum
        {
            delete_material,
            delete_new_flash_content,
            material_download_url,
            new_flash_content,
            content,
            emergency_content,
            timing_boot,
            timing_volume,
        }
    }
}
