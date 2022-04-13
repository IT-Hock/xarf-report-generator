using CommandLine;

namespace ITHock.XarfReportGenerator;

public class CommandLineOptions
{
    [Option('f', "filter", Required = false, HelpText = "Filter file path.", Default = null)]
    public string? Filter { get; set; }

    [Option('s', "start-date", Required = false, HelpText = "Start date to process.",
        Default = null)]
    public DateTime? StartDate { get; set; } = DateTime.Now;

    [Option('e', "end-date", Required = false, HelpText = "End date to process.", Default = null)]
    public DateTime? EndDate { get; set; }
}