using CommandLine;

namespace ITHock.XarfReportGenerator;

public class CommandLineOptions
{
    [Option('c', "config", Required = false, HelpText = "Configuration file path.", Default = "config.json")]
    public string Config { get; set; }

    [Option('o', "output", Required = false, HelpText = "Output folder path.", Default = "xarf")]
    public string Output { get; set; }

    [Option('f', "filter", Required = false, HelpText = "Filter file path.", Default = null)]
    public string? Filter { get; set; }

    [Option('s', "start-date", Required = false, HelpText = "Start date to process.",
        Default = null)]
    public DateTime? StartDate { get; set; } = DateTime.Now;

    [Option('e', "end-date", Required = false, HelpText = "End date to process.", Default = null)]
    public DateTime? EndDate { get; set; }
}