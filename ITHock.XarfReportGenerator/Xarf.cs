using Newtonsoft.Json;

namespace ITHock.XarfReportGenerator;

public class Xarf
{
    private readonly ConfigReporter _reporter;

    public Xarf(Config config)
    {
        _reporter = config.Reporter;
    }

    public string CreateReport(DateTime date, string? sourceIp, ushort sourcePort, string? destIp, ushort destPort,
        string? logMessage)
    {
        var myDeserializedClass = new Root
        {
            ReporterInfo =
            {
                ReporterOrg = _reporter.Organization,
                ReporterOrgDomain = _reporter.Domain,
                ReporterOrgEmail = _reporter.OrganizationEmail,
                ReporterContactEmail = _reporter.ContactEmail,
                ReporterContactName = _reporter.ContactName,
                ReporterContactPhone = _reporter.ContactPhone
            },
            Report =
            {
                ReportClass = "Activity",
                ReportType = "LoginAttack",
                Date = date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"),
                SourceIp = sourceIp,
                SourcePort = sourcePort,
                DestinationIp = destIp,
                DestinationPort = destPort,
                ByteCount = 0,
                PacketCount = 0,
                Ongoing = false,
                Samples = new List<Sample>
                {
                    new()
                    {
                        ContentType = "application/json",
                        Base64Encoded = false,
                        Description = "Log entry",
                        Payload = logMessage
                    }
                }
            }
        };

        return JsonConvert.SerializeObject(myDeserializedClass);
    }

    public string? GetReport(global::ITHock.XarfReportGenerator.Report report)
    {
        return CreateReport(report.DateTime, report.SourceIpAddress, report.SourcePort, report.DestinationIpAddress,
            report.DestinationPort, report.LogEntry);
    }

    [JsonObject]
    public class ReporterInfo
    {
        public string? ReporterOrg { get; set; }
        public string? ReporterOrgDomain { get; set; }
        public string? ReporterOrgEmail { get; set; }
        public string? ReporterContactEmail { get; set; }
        public string? ReporterContactName { get; set; }
        public string? ReporterContactPhone { get; set; }
    }

    [JsonObject]
    public class Sample
    {
        public string? ContentType { get; set; }
        public bool Base64Encoded { get; set; }
        public string? Description { get; set; }
        public string? Payload { get; set; }
    }

    [JsonObject]
    public class Report
    {
        public string? ReportClass { get; set; }
        public string? ReportType { get; set; }

        public string? Date { get; set; }
        public string? SourceIp { get; set; }
        public int SourcePort { get; set; }
        public string? DestinationIp { get; set; }
        public int DestinationPort { get; set; }
        public bool Ongoing { get; set; }
        public int ByteCount { get; set; }
        public int PacketCount { get; set; }
        public List<Sample> Samples { get; set; } = new();
    }

    [JsonObject]
    public class Root
    {
        public string? Version => "2";
        public ReporterInfo ReporterInfo { get; set; } = new();
        public bool Disclosure { get; set; } = false;
        public Report Report { get; set; } = new();
    }
}