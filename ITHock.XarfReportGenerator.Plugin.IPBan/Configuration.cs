using Newtonsoft.Json;

namespace ITHock.XarfReportGenerator.Plugin.IIS;

[JsonObject]
public class Configuration
{
    [JsonProperty("LogDirectory")]
    public string LogDirectory { get; set; } = "C:\\Program Files\\IPBanProPersonal";
}