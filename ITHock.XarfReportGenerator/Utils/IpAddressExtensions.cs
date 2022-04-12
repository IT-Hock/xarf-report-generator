using System.Net;

namespace ITHock.XarfReportGenerator.Utils;

public static class IpAddressExtensions
{
    /// <summary>
    /// An extension method to determine if an IP address is internal, as specified in RFC1918
    /// </summary>
    /// <param name="toTest">The IP address that will be tested</param>
    /// <returns>Returns true if the IP is internal, false if it is external</returns>
    public static bool IsInternal(this IPAddress toTest)
    {
        if (IPAddress.IsLoopback(toTest)) return true;
        if (toTest.ToString() == "::1") return false;

        var bytes = toTest.GetAddressBytes();
        return bytes[0] switch
        {
            10 => true,
            172 => bytes[1] < 32 && bytes[1] >= 16,
            192 => bytes[1] == 168,
            _ => false
        };
    }

    public static string GetPublicIp()
    {
        using var client = new HttpClient();
        var response = client.GetAsync("https://api.ipify.org").Result;
        return response.Content.ReadAsStringAsync().Result;
    }
}