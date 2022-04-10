using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using SimpleLogger;

namespace ITHock.XarfReportGenerator.Plugins;

public static class IpBan
{
    private const string IpBanProDefaultLogPath = @"C:\Program Files\IPBanProPersonal\logfile.txt";
    private const string IpBanFreeDefaultLogPath = @"C:\Program Files\IPBan\logfile.txt";

    private static readonly Regex LineRegex =
        new(@"(?<datetime>\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{4})\|WARN\|IPBan\|(?<message>.*)",
            RegexOptions.Compiled);

    private static readonly Regex MessageRegex =
        new(
            @"Banning ip address: (?<ip>.*), user name: (?<username>.*), config blacklisted: (?<configblacklisted>.*), count: (?<count>.*), extra info: (?<extrainfo>.*), duration: (?<duration>.*)",
            RegexOptions.Compiled);

    public static IEnumerable<Report> GetRecentBans(Config config)
    {
        var ipBanLogFile = config.IpBan.IpBanLogFile;
        if (ipBanLogFile == null || !File.Exists(ipBanLogFile))
        {
            if (!File.Exists(IpBanProDefaultLogPath) && !File.Exists(IpBanFreeDefaultLogPath))
            {
                Logger.Log("No IPBan log file found.");
                return Enumerable.Empty<Report>();
            }

            ipBanLogFile = File.Exists(IpBanProDefaultLogPath) ? IpBanProDefaultLogPath : IpBanFreeDefaultLogPath;
        }

        var logFileLines = File.ReadAllLines(ipBanLogFile);

        var reports = new List<Report>();
        foreach (var logFileLine in logFileLines)
        {
            Logger.Log(Logger.Level.Debug, $"Processing log file line: {logFileLine}");

            var lineMatch = LineRegex.Match(
                logFileLine);
            if (!lineMatch.Success) continue;
            var message = lineMatch.Groups["message"].Value;
            var datetime = DateTime.ParseExact(lineMatch.Groups["datetime"].Value, "yyyy-MM-dd HH:mm:ss.ffff",
                CultureInfo.InvariantCulture);

            var match2 = MessageRegex.Match(message);
            if (!match2.Success) continue;

            var ip = match2.Groups["ip"].Value;
            var username = match2.Groups["username"].Value;
            //var configblacklisted = match2.Groups["configblacklisted"].Value;
            //var count = match2.Groups["count"].Value;
            //var extrainfo = match2.Groups["extrainfo"].Value;
            //var duration = match2.Groups["duration"].Value;

            var report = new Report
            {
                SourceIpAddress = ip,
                Username = username,
                DateTime = datetime,
                SourcePort = 0, // We do not have this information!
                LogEntry = logFileLine,
                DestinationPort = 0, // We do not have this information!
                DestinationIpAddress = config.MyIpAddress
            };
            reports.Add(report);
        }

        return reports;
    }

    public static async Task<IpAddressGeography?> LookupIp(string ip)
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