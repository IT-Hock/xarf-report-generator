using SimpleLogger;
using SimpleLogger.Logging;
using SimpleLogger.Logging.Formatters;
using SimpleLogger.Logging.Handlers;

namespace ITHock.XarfReportGenerator.Utils;

public class SimpleLoggerConsoleLogger : ILoggerHandler
{
    private readonly ILoggerFormatter _loggerFormatter;

    public SimpleLoggerConsoleLogger()
        : this(new SimpleLoggerFormat())
    {
    }

    public SimpleLoggerConsoleLogger(ILoggerFormatter loggerFormatter) => _loggerFormatter = loggerFormatter;

    public void Publish(LogMessage logMessage)
    {
        if (logMessage.Level == Logger.Level.Info)
            Console.ForegroundColor = ConsoleColor.Green;
        else if (logMessage.Level == Logger.Level.Error)
            Console.ForegroundColor = ConsoleColor.DarkRed;
        else if(logMessage.Level == Logger.Level.Warning)
            Console.ForegroundColor = ConsoleColor.DarkYellow;
        else if(logMessage.Level == Logger.Level.Debug)
            Console.ForegroundColor = ConsoleColor.DarkGray;
        else
            Console.ForegroundColor = ConsoleColor.White;

        Console.WriteLine(_loggerFormatter.ApplyFormat(logMessage));
        Console.ResetColor();
    }
}