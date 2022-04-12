using SimpleLogger.Logging;
using SimpleLogger.Logging.Formatters;

namespace ITHock.XarfReportGenerator.Utils;

public class SimpleLoggerFormat : ILoggerFormatter
{
    public string ApplyFormat(LogMessage logMessage)
    {
        var logTagName = logMessage.Level.ToString().ToUpper();
        if(logTagName == "WARNING")
            logTagName = "WARN";
        var logTag = $"[{logTagName}]";
        return
            $" {logTag,7} [{logMessage.DateTime:O}]: {logMessage.Text}";
    }
}