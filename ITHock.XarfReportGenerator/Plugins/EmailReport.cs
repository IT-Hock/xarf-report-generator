using System.Net.Mail;
using ITHock.XarfReportGenerator.Utils;
using SimpleLogger;

namespace ITHock.XarfReportGenerator.Plugins;

public class EmailReport
{
    public static void GenerateReport(Config config, Report ipReport, string abuseContactMail)
    {
        if (!config.EmailReport.Enabled) return;
        if (string.IsNullOrEmpty(config.EmailReport.From) || string.IsNullOrEmpty(config.EmailReport.Subject))
        {
            Logger.Log(Logger.Level.Error, "EmailReport: Invalid configuration. Email report has been disabled.");
            return;
        }

        var mailMessage = new MailMessage();

        mailMessage.From = new MailAddress(config.EmailReport.From);
        mailMessage.To.Add(abuseContactMail);

        if (!string.IsNullOrEmpty(config.EmailReport.Bcc))
            mailMessage.Bcc.Add(config.EmailReport.Bcc);

        mailMessage.Subject = config.EmailReport.Subject;
        mailMessage.Body = GetEmailTemplate(ipReport, config);

        mailMessage.IsBodyHtml = false;
        // AGPLv3 - Do not remove
        mailMessage.Body += "\n\nSend using https://github.com/IT-Hock/xarf-report-generator";

        if (config.EmailReport.AutoReport)
        {
            try
            {
                var smtpServer = new SmtpClient(config.Email.SmtpServer);

                smtpServer.Port = config.Email.Port;
                smtpServer.EnableSsl = config.Email.EnableSsl;

                if (string.IsNullOrEmpty(config.Email.Username))
                    smtpServer.UseDefaultCredentials = true;
                else
                    smtpServer.Credentials =
                        new System.Net.NetworkCredential(config.Email.Username, config.Email.Password);

                smtpServer.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.Level.Error, "Error sending email report: " + ex.Message);
            }
        }
        else
        {
            var filePath = config.EmailReport.ReportOutputPath;
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            filePath = Path.Combine(filePath,
                $"{ipReport.SourceIpAddress}_{ipReport.DateTime:yyyy-MM-dd-HH_mm_ss}.eml");
            File.WriteAllText(filePath, mailMessage.ToEml());
        }
    }

    private static string GetEmailTemplate(Report ipReport, Config config)
    {
        var xarfReporter = new Xarf(config);
        if (string.IsNullOrEmpty(config.EmailReport.ReportTemplate) || !File.Exists(config.EmailReport.ReportTemplate))
        {
            return ipReport + "\n\n" + xarfReporter.GetReport(ipReport);
        }

        var templateContent = File.ReadAllText(config.EmailReport.ReportTemplate);
        if (string.IsNullOrEmpty(templateContent))
            return ipReport + "\n\n" + xarfReporter.GetReport(ipReport);

        templateContent = templateContent.Replace("##XARF_REPORT##", xarfReporter.GetReport(ipReport));
        templateContent = templateContent.Replace("##SOURCE_IP##", ipReport.SourceIpAddress);
        templateContent = templateContent.Replace("##SOURCE_PORT##", ipReport.SourcePort.ToString());
        templateContent = templateContent.Replace("##DEST_IP##", ipReport.DestinationIpAddress);
        templateContent = templateContent.Replace("##DEST_PORT##", ipReport.DestinationPort.ToString());
        templateContent = templateContent.Replace("##TIME##", ipReport.DateTime.ToString("O"));

        return templateContent;
    }
}