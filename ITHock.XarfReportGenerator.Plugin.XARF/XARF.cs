using Newtonsoft.Json;

namespace ITHock.XarfReportGenerator.Plugin.XARF;

public class XARF
{
    public XARF(string organization, string contactPhone, string contactName, string contactEmail,
        string organizationEmail, string domain)
    {
        Organization = organization;
        ContactPhone = contactPhone;
        ContactName = contactName;
        ContactEmail = contactEmail;
        OrganizationEmail = organizationEmail;
        Domain = domain;
    }

    private string Organization { get; }

    private string ContactPhone { get; }

    private string ContactName { get; }

    private string ContactEmail { get; }

    private string OrganizationEmail { get; }

    private string Domain { get; }

    public string CreateReport(DateTime date, string? sourceIp, ushort sourcePort, string? destIp, ushort destPort,
        string? logMessage)
    {
        var myDeserializedClass = new XarfRoot
        {
            ReporterInfo =
            {
                ReporterOrg = Organization,
                ReporterOrgDomain = Domain,
                ReporterOrgEmail = OrganizationEmail,
                ReporterContactEmail = ContactEmail,
                ReporterContactName = ContactName,
                ReporterContactPhone = ContactPhone
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

    public string GetReport(Report report)
    {
        return CreateReport(report.DateTime, report.SourceIpAddress, report.SourcePort, report.DestinationIpAddress,
            report.DestinationPort, report.LogEntry);
    }

    [JsonObject]
    public class XarfReporterInfo
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
    public class XarfReport
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
    public class XarfRoot
    {
        [JsonProperty("Version")]
        public string Version => "2";

        [JsonProperty("ReporterInfo")]
        public XarfReporterInfo ReporterInfo { get; set; } = new();

        [JsonProperty("Disclosure")]
        public bool Disclosure { get; set; } = false;

        [JsonProperty("Report")]
        public XarfReport Report { get; set; } = new();
    }
}