using Newtonsoft.Json;

namespace ITHock.XarfReportGenerator;

[JsonObject]
public class ConfigReporter
{
    public string Organization { get; set; } = "?";
    public string Domain { get; set; } = "?";
    public string ContactEmail { get; set; } = "?";
    public string ContactName { get; set; } = "?";
    public string ContactPhone { get; set; } = "?";
    public string OrganizationEmail { get; set; } = "?";
}

[JsonObject]
public class ConfigDefaultPlugin
{
    public bool Enabled { get; set; } = true;
}

[JsonObject]
public class ConfigEventViewer : ConfigDefaultPlugin
{
}

[JsonObject]
public class ConfigIpBan : ConfigDefaultPlugin
{
    public string? IpBanLogFile { get; set; }
}

[JsonObject]
public class ConfigEmailReport : ConfigDefaultPlugin
{
    public string From { get; set; } = "admin@localhost";
    public string? Bcc { get; set; }
    public string? Subject { get; set; } = "RDP Brute-Force Report";

    public string? ReportTemplate { get; set; }
    public bool AutoReport { get; set; }

    public string ReportOutputPath { get; set; } = "EmailReport";
}

[JsonObject]
public class ConfigEmail
{
    public string SmtpServer { get; set; } = "localhost";
    public ushort Port { get; set; } = 25;
    public bool EnableSsl { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}

[JsonObject]
public class Config
{
    public ConfigReporter Reporter { get; set; } = new();
    public ConfigEmail Email { get; set; } = new();
    public string MyIpAddress { get; set; } = "0.0.0.0";

    public ConfigEventViewer EventViewer { get; set; } = new();
    public ConfigIpBan IpBan { get; set; } = new();
    public ConfigEmailReport EmailReport { get; set; } = new();
    public string OutputPath { get; set; } = "xarf";

    public static Config? Load(string filePath)
    {
        return !File.Exists(filePath) ? null : JsonConvert.DeserializeObject<Config>(File.ReadAllText(filePath));
    }
}