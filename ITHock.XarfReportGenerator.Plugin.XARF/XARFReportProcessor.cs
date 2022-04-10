namespace ITHock.XarfReportGenerator.Plugin.XARF;

public class XARFReportProcessor : IReportProcessor
{
    private string Organization { get; }

    private string ContactPhone { get; }

    private string ContactName { get; }

    private string ContactEmail { get; }

    private string OrganizationEmail { get; }

    private string Domain { get; }

    private string OutputDirectory { get; }

    public XARFReportProcessor(IPlugin plugin)
    {
        // TODO: Read configuration from plugin
        Organization = "TODO";
        ContactPhone = "TODO";
        ContactName = "TODO";
        ContactEmail = "TODO";
        OrganizationEmail = "TODO";
        Domain = "TODO";

        OutputDirectory = "xarf";
    }

    public bool ProcessReport(Report report)
    {
        var xarf = new XARF(Organization, ContactPhone, ContactName, ContactEmail, OrganizationEmail, Domain);
        var xarfReport = xarf.GetReport(report);
        if (string.IsNullOrEmpty(xarfReport))
            return false;

        var filePath = Path.Combine(OutputDirectory,
            $"{report.SourceIpAddress}_{report.DateTime:dd_MM_yyyy-HH_mm_ss}.json");
        if (!Directory.Exists(OutputDirectory))
        {
            Directory.CreateDirectory(OutputDirectory);
        }

        File.WriteAllText(filePath, xarfReport);
        return true;
    }
}