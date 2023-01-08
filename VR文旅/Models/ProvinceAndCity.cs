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
        [JsonPropertyName("locations")]
        public Dictionary<string, City> Provinces { get; set; } = new();
        public bool Success { get; set; }
        public static async Task<bool> GetProvinceAndCityAsync()
        {
            var request = new
            {
                org_id = 1,
            };
            var response = await request.PostAsync<ProvinceAndCity>(PATH);
            _locations.Clear();
            if (response.Success)
                foreach (var province in response.Provinces)
                {
                    _locations.Add(new Location(province.Key, ""), false);
                    foreach (var city in province.Value.Citys)
                        _locations.Add(new Location(province.Key, city), false);
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
        public static Location[] GetLocations()
        {
            var result = new List<Location>();
            foreach (var item in _locations.Where(x=>x.Value))
                result.Add(item.Key);
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
