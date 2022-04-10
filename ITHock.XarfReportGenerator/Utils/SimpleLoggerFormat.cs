using SimpleLogger;
using SimpleLogger.Logging;
using SimpleLogger.Logging.Formatters;

namespace ITHock.XarfReportGenerator.Utils;

public class SimpleLoggerFormat : ILoggerFormatter
{
    public string ApplyFormat(LogMessage logMessage)
    {
        var loggerTag = $"[{logMessage.Level.ToString().ToUpper()}]";
        return
            $" {loggerTag,-7} [{logMessage.DateTime:O}]: {logMessage.Text}";
    }
}