namespace VR文旅.Models.Requests
{
    internal class Access: IRequest
    {
        public Access()
        {
            MacAddress = RSAEncrypt(Global.PUBLIC_KEY, GetMacAddress());
        }
        [JsonPropertyName("mac_address")]
        public string MacAddress { get; set; }
        [JsonIgnore]
        public string Url => "/client/access";

        private static string GetMacAddress()
        {
            var result= "00-00-00-00-00-00";
            try
            {
                var interfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
                foreach (var ni in interfaces)
                {
                    result= BitConverter.ToString(ni.GetPhysicalAddress().GetAddressBytes());
                }
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex, "GetMacAddress");
            }
            return result;
        }
        private static string RSAEncrypt(string publicKey,string encryptString)
        {
            System.Security.Cryptography.RSACryptoServiceProvider res = new();
            res.FromXmlString(publicKey);
            byte[] data = new System.Text.UnicodeEncoding().GetBytes(encryptString);
            var resultBytes = res.Encrypt(data, false);
            return Convert.ToBase64String(resultBytes);
        }
    }
}
