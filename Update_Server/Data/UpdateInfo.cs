using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Update_Server.Data
{
    public class UpdateInfo
    {
        //
        // 摘要:
        //     Download URL of the update file.
        [XmlElement("url")]
        public string DownloadURL { get; set; }

        //
        // 摘要:
        //     URL of the webpage specifying changes in the new update.
        [XmlElement("changelog")]
        public string ChangelogURL { get; set; }

        //
        // 摘要:
        //     Returns newest version of the application available to download.
        [XmlElement("version")]
        public string CurrentVersion { get; set; }

        //
        // 摘要:
        //     Returns version of the application currently installed on the user's PC.
        public Version InstalledVersion { get; set; }

        //
        // 摘要:
        //     Shows if the update is required or optional.
        [XmlElement("mandatory")]
        public Mandatory Mandatory { get; set; }

        //
        // 摘要:
        //     Command line arguments used by Installer.
        [XmlElement("args")]
        public string InstallerArgs { get; set; }

        //
        // 摘要:
        //     Checksum of the update file.
        [XmlElement("checksum")]
        public CheckSum CheckSum { get; set; }
    }
    public class Mandatory
    {
        //
        // 摘要:
        //     Value of the Mandatory field.
        [XmlText]
        public bool Value { get; set; }

        //
        // 摘要:
        //     If this is set and 'Value' property is set to true then it will trigger the mandatory
        //     update only when current installed version is less than value of this property.
        [XmlAttribute("minVersion")]
        public string MinimumVersion { get; set; }

        //
        // 摘要:
        //     Mode that should be used for this update.
        [XmlAttribute("mode")]
        public ModeEnum UpdateMode { get; set; }
        public enum ModeEnum
        {
            //
            // 摘要:
            //     In this mode, it ignores Remind Later and Skip values set previously and hide
            //     both buttons.

            Normal,
            //
            // 摘要:
            //     In this mode, it won't show close button in addition to Normal mode behaviour.

            Forced,
            //
            // 摘要:
            //     In this mode, it will start downloading and applying update without showing standard
            //     update dialog in addition to Forced mode behaviour.

            ForcedDownload
        }
    }
    public class CheckSum
    {
        //
        // 摘要:
        //     Hash of the file.
        [XmlText]
        public string Value { get; set; }

        //
        // 摘要:
        //     Hash algorithm that generated the hash.
        [XmlAttribute("algorithm")]
        public string HashingAlgorithm { get; set; }
    }
}
