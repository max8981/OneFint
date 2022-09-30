using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Update_Server.Data
{
    public record UpdateInfo
    {
        
        public string CurrentVersion { get; set; }
        
        public string DownloadURL { get; set; }
        
        public string ChangelogURL { get; set; }
        
        public Mandatory Mandatory { get; set; }
        
        public Checksum Checksum { get; set; }
    }
    public record Mandatory
    {
        
        public bool Value { get; set; }
        
        public string MinVersion { get; set; }
        
        public ModeEnum Mode { get; set; }
    }
    public record Checksum
    {
        
        public string Value { get; set; }
        
        public string HashingAlgorithm { get; set; }
    }
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
