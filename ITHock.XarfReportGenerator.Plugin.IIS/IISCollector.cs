using Tx.Windows;

namespace ITHock.XarfReportGenerator.Plugin.IIS;

public class IISCollector : IReportCollector
{
    private static readonly string _defaultLogPath = @"C:\inetpub\logs";
    public IEnumerable<Report> GatherReports()
    {
        var reports = new List<Report>();
        var logFiles = Directory.GetFiles(_defaultLogPath, "*.log", SearchOption.AllDirectories);
        foreach (var logFile in logFiles)
        {
            var logEntries = W3CEnumerable.FromFile(logFile);
            foreach (var logEntry in logEntries)
            {
                var query = (logEntry.cs_uri_query != "" ? "?" + logEntry.cs_uri_query : "-");
                var uri =
                    $"{logEntry.c_ip} {logEntry.cs_username} {query} [{logEntry.dateTime:O}] \"{logEntry.cs_method} {logEntry.cs_uri_stem} HTTP/{logEntry.cs_version}\" {logEntry.sc_status} {logEntry.sc_bytes} \"{logEntry.cs_Referer}\" \"{logEntry.cs_User_Agent}\"";

                var report = new Report
                {
                    SourceIpAddress = logEntry.c_ip,
                    LogEntry = uri,
                    DestinationPort = ushort.Parse(logEntry.s_port)
                };
                reports.Add(report);
            }
        }
        
        return reports;
    }
}