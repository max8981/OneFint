using LiteDB;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Xml;

namespace Update_Server.Data
{
    public class Project
    {
        public Project()
        {
            Packages = new List<Package>();
            Id = ObjectId.NewObjectId();
        }
        private const string DBPath = "./data.db";
        public ObjectId Id { get; set; }
        public string? Name { get; set; }
        public string? Key { get; set; }
        public ICollection<Package> Packages { get; set; }
        [BsonIgnore]
        public UpdateInfo UpdateInfo => GetUpdateInfo();
        [BsonIgnore]
        public string Xml => GetXmlString();
        public void Save()
        {
            using var db = new LiteDatabase(DBPath);
            var col = db.GetCollection<Project>(nameof(Project));
            if(col.Find(_ => _.Id == Id).Any())
            {
                try
                {
                    col.Update(this);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                col.Insert(this);
            }
        }
        public void Delete()
        {
            using var db = new LiteDatabase(DBPath);
            var col = db.GetCollection<Project>(nameof(Project));
            col.Delete(Id);
        }
        private XmlDocument BuildeXml(Package package)
        {
            XmlDocument xml = new();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
            var xml_item = xml.CreateElement("", "item", "");
            var xml_version = xml.CreateElement("version");
            xml_version.InnerText = package.Version;
            var xml_url = xml.CreateElement("url");
            xml_url.InnerText = $"http://yuxtech.com:20000/api/update/GetPackage?key={Key}";
            var xml_changelog = xml.CreateElement("changelog");
            xml_changelog.InnerText = $"http://yuxtech.com:20000/Changelog?key={Key}";
            var xml_mandatory = xml.CreateElement("mandatory");
            xml_mandatory.SetAttribute("minVersion", package.MinimumVersion);
            xml_mandatory.SetAttribute("mode", "2");
            xml_mandatory.InnerText = package.Mandatory.ToString();
            var xml_checksum = xml.CreateElement("checksum");
            xml_checksum.SetAttribute("algorithm", "MD5");
            xml_checksum.InnerText = package.MD5;
            xml_item.AppendChild(xml_version);
            xml_item.AppendChild(xml_url);
            xml_item.AppendChild(xml_changelog);
            xml_item.AppendChild(xml_mandatory);
            xml_item.AppendChild(xml_checksum);
            xml.AppendChild(xml_item);
            xml.Save("./test.xml");
            return xml;
        }
        private UpdateInfo GetUpdateInfo()
        {
            var result = new UpdateInfo();
            if (Packages.Count > 0)
            {
                var package = Packages.OrderByDescending(_ => _.Version).First();
                result.CurrentVersion = package.Version;
                result.DownloadURL = $"http://yuxtech.com:20000/api/update/GetPackage?key={Key}";
                result.ChangelogURL = $"http://yuxtech.com:20000/Changelog?key={Key}";
                result.Mandatory = new Data.Mandatory
                {
                    MinimumVersion = package.MinimumVersion,
                    UpdateMode = Data.Mandatory.ModeEnum.Forced,
                    Value = package.Mandatory,
                };
                result.CheckSum = new Data.CheckSum
                {
                    HashingAlgorithm = "MD5",
                    Value = package.MD5,
                };
            }
            return result;
        }
        private string GetXmlString()
        {
            XmlDocument result = new();
            if (Packages.Count > 0)
            {
                var package = Packages.OrderByDescending(_ => _.Version).First();
                result = BuildeXml(package);
            }
            return result.InnerXml;
        }
        public static Project GetProject(ObjectId id)
        {
            using var db = new LiteDatabase(DBPath);
            var col = db.GetCollection<Project>(nameof(Project));
            return col.Find(_ => _.Id == id).FirstOrDefault() ?? new Project();
        }
        public static Project GetProject(string key)
        {
            using var db = new LiteDatabase(DBPath);
            var col = db.GetCollection<Project>(nameof(Project));
            return col.FindOne(_ => _.Key == key) ?? new Project();
        }
        public static Project[] GetProjects()
        {
            using var db = new LiteDatabase(DBPath);
            var col = db.GetCollection<Project>(nameof(Project));
            return col.Query().ToArray();
        }
    }
}
