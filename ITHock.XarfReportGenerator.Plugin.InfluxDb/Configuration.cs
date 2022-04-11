using Newtonsoft.Json;

namespace ITHock.XarfReportGenerator.Plugin.InfluxDb;

[JsonObject]
public class Configuration
{
    [JsonProperty("InfluxUrl")]
    public string InfluxUrl { get; set; } = "http://localhost:8086";

    [JsonProperty("InfluxDbName")]
    public string InfluxDbName { get; set; } = "data";

    [JsonProperty("InfluxDbUser")]
    public string? InfluxDbUser { get; set; }

    [JsonProperty("InfluxDbPassword")]
    public string? InfluxDbPassword { get; set; }
}