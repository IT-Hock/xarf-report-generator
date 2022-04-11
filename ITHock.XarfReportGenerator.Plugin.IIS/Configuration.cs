using Newtonsoft.Json;

namespace ITHock.XarfReportGenerator.Plugin.IIS;

[JsonObject]
public class Configuration
{
    [JsonProperty("StatusCodeFilter")]
    public string? StatusCodeFilter { get; set; } = "403|405|406|400";
    
    [JsonProperty("MethodFilter")]
    public string? MethodFilter { get; set; } = "POST|PUT|DELETE|HEAD|OPTIONS|TRACE|CONNECT";
    
    [JsonProperty("PathFilter")]
    public string? PathFilter { get; set; }
}