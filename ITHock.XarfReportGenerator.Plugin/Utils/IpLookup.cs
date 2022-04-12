using Newtonsoft.Json;

namespace ITHock.XarfReportGenerator.Plugin.Utils;

public class IpLookup
{
    [JsonObject]
    private class IpApi_Result
    {
        [JsonProperty("query")]
        public string Query { get; set; } = "";

        [JsonProperty("status")]
        public string Status { get; set; } = "error";

        [JsonProperty("country")]
        public string Country { get; set; } = "";

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; } = "";

        [JsonProperty("region")]
        public string Region { get; set; } = "";

        [JsonProperty("regionName")]
        public string RegionName { get; set; } = "";

        [JsonProperty("city")]
        public string City { get; set; } = "";

        [JsonProperty("zip")]
        public string Zip { get; set; } = "";

        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lon")]
        public double Lon { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; } = "";

        [JsonProperty("isp")]
        public string Isp { get; set; } = "";

        [JsonProperty("org")]
        public string Org { get; set; } = "";

        [JsonProperty("as")]
        public string As { get; set; } = "";
    }
    
    public static async Task<IpAddressGeography?> IpApi_LookupIp(string ip)
    {
        using var client = new HttpClient();
        var geography = await await client.GetAsync($"http://ip-api.com/json/{ip}").ContinueWith(async requestTask =>
        {
            var response = await requestTask;
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IpApi_Result>(json);
            return result;
        });

        if (geography?.Status == "success")
            return new IpAddressGeography
            {
                Error = false,
                Geography = new IpAddressGeography.IpGeography
                {
                    Continent = "",
                    ContinentCode = "",
                    Country = geography.Country,
                    CountryCode = geography.CountryCode,
                    ISP = geography.Isp,
                    Latitude = geography.Lat,
                    Longitude = geography.Lon,
                    LocationAccuracyRadius = 0
                },
                IPAddress = ip
            };

        return null;
    }
    
    public static async Task<IpAddressGeography?> IpBan_LookupIp(string ip)
    {
        using var client = new HttpClient();
        var geography = await client.GetAsync($"https://api.ipban.com/ip/{ip}").ContinueWith(async requestTask =>
        {
            var response = await requestTask;
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IpAddressGeography>(json);
            return result;
        });

        return await geography;
    }
}