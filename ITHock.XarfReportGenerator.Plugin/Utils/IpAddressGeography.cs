namespace ITHock.XarfReportGenerator.Plugin.Utils;

public class IpAddressGeography
{
    public class IpGeography
    {
        public string Country { get; set; } = "-";
        public string CountryCode { get; set; } = "-";
        public string Continent { get; set; } = "-";
        public string ContinentCode { get; set; } = "-";
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int LocationAccuracyRadius { get; set; }
        public string ISP { get; set; } = "-";
    }
    public string IPAddress { get; set; } = "0.0.0.0";
    public bool Error { get; set; }
    
    public IpGeography Geography { get; set; } = new();

    public string? AbuseEmail { get; set; }
}