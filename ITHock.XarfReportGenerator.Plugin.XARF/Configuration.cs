using Newtonsoft.Json;

namespace ITHock.XarfReportGenerator.Plugin.XARF;

[JsonObject]
public class Configuration
{
    [JsonProperty("EmailReportOutput")]
    public string? EmailReportOutputPath { get; set; }
        
    [JsonProperty("EmailReportTemplate")]
    public string? EmailReportTemplate { get; set; }
        
    [JsonProperty("BccMail")]
    public string? Bcc_Mail { get; set; }

    [JsonProperty("FromMail")]
    public string From_Mail { get; set; } = "admin@local.host";

    [JsonProperty("Organization")]
    public string Organization { get; set; }

    [JsonProperty("ContactPhone")]
    public string ContactPhone { get; set; }

    [JsonProperty("ContactName")]
    public string ContactName { get; set; }

    [JsonProperty("ContactEmail")]
    public string ContactEmail { get; set; }

    [JsonProperty("OrganizationEmail")]
    public string OrganizationEmail { get; set; }

    [JsonProperty("Domain")]
    public string Domain { get; set; }

    [JsonProperty("OutputDirectory")]
    public string OutputDirectory { get; set; } = "xarf";
}