using System.Net.Mail;
using System.Reflection;
using System.Text;

namespace ITHock.XarfReportGenerator.Utils;

public static class MailMessageExtensions
{
    public static string ToEml(this MailMessage message)
    {
        var assembly = typeof(SmtpClient).Assembly;
        var mailWriterType = assembly.GetType("System.Net.Mail.MailWriter");
        if (mailWriterType == null)
            throw new Exception("Could not find type: System.Net.Mail.MailWriter");
        
        using var memoryStream = new MemoryStream();

        // Get reflection info for MailWriter contructor
        var mailWriterConstructor =
            mailWriterType.GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new[] { typeof(Stream), typeof(bool) },
                null);
        if(mailWriterConstructor == null)
            throw new Exception("Could not find constructor: System.Net.Mail.MailWriter(System.IO.Stream, bool)");

        // Construct MailWriter object with our FileStream
        var mailWriter =
            mailWriterConstructor.Invoke(new object[] { memoryStream, true });
        if(mailWriter == null)
            throw new Exception("Could not construct MailWriter");

        // Get reflection info for Send() method on MailMessage
        var sendMethod =
            typeof(MailMessage).GetMethod(
                "Send",
                BindingFlags.Instance | BindingFlags.NonPublic);
        if(sendMethod == null)
            throw new Exception("Could not find method: Send(System.Net.Mail.MailWriter, bool, bool)");

        // Call method passing in MailWriter
        sendMethod.Invoke(
            message,
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            new[] { mailWriter, true, true },
            null);

        // Finally get reflection info for Close() method on our MailWriter
        var closeMethod =
            mailWriter.GetType().GetMethod(
                "Close",
                BindingFlags.Instance | BindingFlags.NonPublic);
        if(closeMethod == null)
            throw new Exception("Could not find method: Close()");

        // Call close method
        closeMethod.Invoke(
            mailWriter,
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            Array.Empty<object>(),
            null);
        
        return Encoding.ASCII.GetString(memoryStream.ToArray());
    }
}