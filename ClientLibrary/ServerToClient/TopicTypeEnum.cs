using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary.ServerToClient
{
    public enum TopicTypeEnum
    {
        delete_material,
        delete_new_flash_content,
        material_download_url,
        new_flash_content,
        content,
        emergency_content,
        timing_boot,
        timing_volume,
        order,

        screen_control,
        time_sync,
    }
}
