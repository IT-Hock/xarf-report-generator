namespace ITHock.XarfReportGenerator;

public class Geography
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

public class IpAddressGeography
{
    public string IPAddress { get; set; } = "0.0.0.0";
    public bool Error { get; set; }
    public Geography Geography { get; set; } = new();

    public string? AbuseEmail { get; set; }
}