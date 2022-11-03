using LiteDB;

namespace Update_Server.Data
{
    public class Package
    {
        public Guid Id { get; set; }
        public string Version { get; set; }
        public string[] Changelogs { get; set; }
        public bool Mandatory { get; set; }
        public string MinimumVersion { get; set; }
        public string PackagePath { get; set; }
        public string MD5 { get; set; }
        public DateTime CreateDate { get; set; }
        //public Project Project { get; set; }
        public ObjectId ProjectId { get; set; }
    }
}
