namespace ITHock.XarfReportGenerator.Plugin.XARF;

public class XARFReportProcessor : IReportProcessor
{
    private XARFPlugin _plugin;

    public XARFReportProcessor(IPlugin plugin)
    {
        _plugin = (XARFPlugin)plugin;
    }

    public bool ProcessReport(Report report)
    {
        var xarf = new XARF(_plugin.Config.Organization, _plugin.Config.ContactPhone, _plugin.Config.ContactName,
            _plugin.Config.ContactEmail, _plugin.Config.OrganizationEmail, _plugin.Config.Domain);
        var xarfReport = xarf.GetReport(report);
        if (string.IsNullOrEmpty(xarfReport))
            return false;

        var filePath = Path.Combine(_plugin.Config.OutputDirectory,
            $"{report.SourceIpAddress}_{report.DateTime:dd_MM_yyyy-HH_mm_ss}.json");
        if (!Directory.Exists(_plugin.Config.OutputDirectory))
        {
            Directory.CreateDirectory(_plugin.Config.OutputDirectory);
        }

        File.WriteAllText(filePath, xarfReport);
        return true;
    }
}