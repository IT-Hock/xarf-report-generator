using Newtonsoft.Json;

namespace ITHock.XarfReportGenerator.AbuseIpDb;

public class IpLookup
{
    public static async Task<IpAddressGeography?> LookupIp(string ip)
    {
        using var client = new HttpClient();
        var geography = await await client.GetAsync($"http://ip-api.com/json/{ip}").ContinueWith(async requestTask =>
        {
            var response = await requestTask;
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Root>(json);
            return result;
        });

        if (geography?.Status == "success")
            return new IpAddressGeography
            {
                Error = false,
                Geography = new Geography
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

    [JsonObject]
    private class Root
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
}