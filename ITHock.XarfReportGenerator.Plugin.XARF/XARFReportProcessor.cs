using System.Net.Mail;
using SimpleLogger;

namespace ITHock.XarfReportGenerator.Plugin.XARF;

public class XARFReportProcessor : IReportProcessor
{
    private XARFPlugin? _plugin;

    public XARFReportProcessor(IPlugin plugin)
    {
        _plugin = (XARFPlugin?)plugin;
    }

    public bool ProcessReport(Report report)
    {
        if (_plugin == null || !_plugin.IsInitialized)
        {
            Logger.Log(Logger.Level.Error, "[XARFPlugin] Plugin not initialized");
            return false;
        }

        if (_plugin.Config == null)
        {
            Logger.Log(Logger.Level.Error, "[XARFPlugin] Plugin configuration not set");
            return false;
        }

        var xarf = new XARF(_plugin.Config.Organization, _plugin.Config.ContactPhone, _plugin.Config.ContactName,
            _plugin.Config.ContactEmail, _plugin.Config.OrganizationEmail, _plugin.Config.Domain);
        var xarfReport = xarf.GetReport(report);
        if (string.IsNullOrEmpty(xarfReport))
            return false;
        
        if (!GenerateXarfReport(report, xarfReport)) return false;
        if (!GenerateEmailReport(report, xarfReport)) return false;
        return true;
    }

    private bool GenerateEmailReport(Report report, string xarfReport)
    {
        if(report.SourceIpAddressGeography?.AbuseEmail == null)
            return false;
        
        var mailMessage = new MailMessage();

        mailMessage.From = new MailAddress(_plugin.Config.From_Mail);
        mailMessage.To.Add(report.SourceIpAddressGeography.AbuseEmail);

        if (!string.IsNullOrEmpty(_plugin.Config.Bcc_Mail))
            mailMessage.Bcc.Add(_plugin.Config.Bcc_Mail);

        mailMessage.Subject = $"XARF Report Abuse from {report.SourceIpAddress}";
        mailMessage.Body = GetEmailTemplate(report, xarfReport);

        mailMessage.IsBodyHtml = false;
        // AGPLv3 - Do not remove
        mailMessage.Body += "\n\nSend using https://github.com/IT-Hock/xarf-report-generator";
        
        var filePath = _plugin.Config.OutputDirectory;
        if (_plugin.Config.EmailReportOutputPath != null)
        {
            filePath = _plugin.Config.EmailReportOutputPath;
        }
        else
        {
            if (report.SourceIpAddressGeography != null)
            {
                if (!string.IsNullOrEmpty(report.SourceIpAddressGeography.Geography.ISP))
                {
                    filePath = Path.Combine(filePath, StringExtensions.GetCleanFileName(report.SourceIpAddressGeography.Geography.ISP));
                }
                else
                    filePath = Path.Combine(filePath,
                        StringExtensions.GetCleanFileName(report.SourceIpAddressGeography.Geography.CountryCode));
            }
        }

        if (!Directory.Exists(filePath))
            Directory.CreateDirectory(filePath);

        filePath = Path.Combine(filePath,
            $"{report.SourceIpAddress}_{report.DateTime:yyyy-MM-dd-HH_mm_ss}.eml");
        File.WriteAllText(filePath, mailMessage.ToEml());
        
        return true;
    }

    private bool GenerateXarfReport(Report report, string xarfReport)
    {
        var filePath = _plugin.Config.OutputDirectory;
        if (report.SourceIpAddressGeography != null)
        {
            if (!string.IsNullOrEmpty(report.SourceIpAddressGeography.Geography.ISP))
            {
                filePath = Path.Combine(filePath,
                    StringExtensions.GetCleanFileName(report.SourceIpAddressGeography.Geography.ISP));
            }
            else
                filePath = Path.Combine(filePath,
                    StringExtensions.GetCleanFileName(report.SourceIpAddressGeography.Geography.CountryCode));
        }

        if (!Directory.Exists(filePath))
            Directory.CreateDirectory(filePath);

        filePath = Path.Combine(filePath,
            $"{report.SourceIpAddress}_{report.DateTime:yyyy-MM-dd-HH_mm_ss}.json");

        if (!Directory.Exists(_plugin.Config.OutputDirectory))
        {
            Directory.CreateDirectory(_plugin.Config.OutputDirectory);
        }

        File.WriteAllText(filePath, xarfReport);
        return true;
    }
    
    private string GetEmailTemplate(Report ipReport, string xarfReport)
    {
        if (string.IsNullOrEmpty(_plugin.Config.EmailReportTemplate) || !File.Exists(_plugin.Config.EmailReportTemplate))
            return ipReport + "\n\n" + xarfReport;

        var templateContent = File.ReadAllText(_plugin.Config.EmailReportTemplate);
        if (string.IsNullOrEmpty(templateContent))
            return ipReport + "\n\n" + xarfReport;

        templateContent = templateContent.Replace("##XARF_REPORT##", xarfReport);
        templateContent = templateContent.Replace("##SOURCE_IP##", ipReport.SourceIpAddress);
        templateContent = templateContent.Replace("##SOURCE_PORT##", ipReport.SourcePort.ToString());
        templateContent = templateContent.Replace("##DEST_IP##", ipReport.DestinationIpAddress);
        templateContent = templateContent.Replace("##DEST_PORT##", ipReport.DestinationPort.ToString());
        templateContent = templateContent.Replace("##TIME##", ipReport.DateTime.ToString("O"));

        return templateContent;
    }
}