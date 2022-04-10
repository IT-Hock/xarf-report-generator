using System.Globalization;
using System.Text.RegularExpressions;

namespace ITHock.XarfReportGenerator.Plugin.nginx;

public class NginxCollector : IReportCollector
{
    private readonly string _nginxDefaultLogDir = @"C:\Program Files\nginx\logs";

    private readonly Regex _nginxLineRegex =
        new(@"(\d+\.\d+\.\d+\.\d+)\s.*?\s.*?\s.*?\[(.*?)\]\s\""(.*?)\""\s\d+\s\d+\s.*?\s.*?$");

    public IEnumerable<Report> GatherReports()
    {
        var reports = new List<Report>();

        var logFiles = Directory.GetFiles(_nginxDefaultLogDir, "*.log", SearchOption.AllDirectories);
        foreach (var logFile in logFiles)
        {
            var lines = File.ReadAllLines(logFile);
            foreach (var line in lines)
            {
                var match = _nginxLineRegex.Match(line);
                if (!match.Success) continue;

                var dateRegex = new Regex(@"\d{2}/[a-zA-Z]{3}/\d{4}:\d{2}:\d{2}:\d{2}");
                var ip = match.Groups[1].Value;
                var dateMatch = dateRegex.Match(match.Groups[2].Value);
                var date = dateMatch.Groups[0].Value;
                var url = match.Groups[3].Value;
                var dateTime = DateTime.ParseExact(date, "dd/MMM/yyyy:HH:mm:ss", CultureInfo.InvariantCulture);

                reports.Add(new Report
                {
                    DateTime = dateTime,
                    SourceIpAddress = ip,
                });
            }
        }

        return reports;
    }
}