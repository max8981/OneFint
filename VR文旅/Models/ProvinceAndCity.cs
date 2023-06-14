using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VR文旅.Models.ProvinceAndCity;

namespace VR文旅.Models
{
    internal class ProvinceAndCity:Http.IResponse
    {
        private const string PATH = "/api/client/province_and_city/";
        private static readonly Dictionary<Location, bool> _locations = new();
        private static readonly Dictionary<string, City> _provinceAndCity = new();
        [JsonPropertyName("locations")]
        public Dictionary<string, City> Provinces { get; set; } = new();
        public bool Success { get; set; }
        public static async Task<bool> GetProvinceAndCityAsync()
        {
            var request = new
            {
                org_id = Systems.Config.OrdId,
            };
            var response = await request.PostAsync<ProvinceAndCity>(PATH);
            _locations.Clear();
            _provinceAndCity.Clear();
            if (response.Success)
                foreach (var province in response.Provinces)
                {
                    _locations.Add(new Location(province.Key, ""), false);
                    foreach (var city in province.Value.Citys)
                        _locations.Add(new Location(province.Key, city), false);
                    _provinceAndCity.Add(province.Key, province.Value);
                }
            return response.Success;
        }
        public static Dictionary<Location, bool> GetProvinces()
        {
            var result = new Dictionary<Location, bool>();
            foreach (var item in _locations)
                if (item.Key.City == "")
                    result.Add(item.Key, item.Value);
            return result;
        }
        public static Dictionary<Location, bool> GetCitys()
        {
            var result = new Dictionary<Location, bool>();
            var provinces = GetProvinces();
            foreach (var province in provinces)
                if (province.Value|| provinces.All(x=>!x.Value))
                    foreach (var city in _locations.Where(x => x.Key.Province == province.Key.Province).Where(x => x.Key.City != "").ToArray())
                        result.Add(city.Key, city.Value);
            return result;
        }
        public static string[] GetProvinceList => _provinceAndCity.Keys.ToArray();
        public static string[] GetCityList(string[] provinces)
        {
            var result = new List<string>();
            if (provinces.Any())
                foreach (var province in provinces)
                    result.AddRange(_provinceAndCity[province].Citys);
            else
                foreach (var item in _provinceAndCity)
                    result.AddRange(item.Value.Citys);
            return result.ToArray();
        }
        public static Location[] GetLocations()
        {
            var result = new List<Location>();
            foreach (var item in _locations.Where(x=>x.Value))
                result.Add(item.Key);
            return result.ToArray();
        }
        public static Location[] GetLocations(string[] provinces, string[] citys)
        {
            var result = new List<Location>();
            foreach (var province in provinces)
                foreach (var city in _provinceAndCity[province].Citys)
                    if (!citys.Any()||citys.Contains(city))
                        result.Add(new Models.Location { Province = province, City = city });
            return result.ToArray();
        }
        public static void SetValue(Location location,bool value)
        {
            if (location.City == "")
                if (!value)
                    foreach (var item in _locations)
                        if (item.Key.Province == location.Province)
                            _locations[item.Key] = value;
            _locations[location] = value;
        }
        public static Dictionary<Location, bool> Location => _locations;
        internal struct City
        {
            [JsonPropertyName("citys")]
            public string[] Citys { get; set; }
        }
    }
}
