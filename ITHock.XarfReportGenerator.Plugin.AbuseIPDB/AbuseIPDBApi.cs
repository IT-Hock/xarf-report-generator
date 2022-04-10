using System.Net;
using System.Net.Http.Headers;

namespace ITHock.XarfReportGenerator.Plugin.AbuseIPDB;

public class AbuseIPDBApi
{
    public enum Categories
    {
        Fraud_Orders = 3,
        DDoS_Attack = 4,
        Open_Proxy = 9,
        Web_Spam = 10,
        Email_Spam = 11,
        Port_Scan = 14,
        Hacking = 15,
        Brute_Force = 18,
        Bad_Web_Bot = 19,
        Exploited_Host = 20,
        Web_App_Attack = 21,
        SSH = 22,
        IoT_Targeted = 23
    }

    public static async Task<bool> ReportIp(string apiKey, string ip, string comment,
        IEnumerable<Categories> categories)
    {
        var intCats = categories.Aggregate("", (current, cat) => current + $"{(int)cat},");
        var commentEncoded = WebUtility.UrlEncode(comment);
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Key", apiKey);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var result = await client.PostAsync(
            $"https://api.abuseipdb.com/api/v2/report?ip={ip}&comment={commentEncoded}&categories={intCats}",
            new StringContent($"categories={intCats}"));
        var body = await result.Content.ReadAsStringAsync();
        
        return result.StatusCode == HttpStatusCode.OK && body.Contains(ip);
    }
}