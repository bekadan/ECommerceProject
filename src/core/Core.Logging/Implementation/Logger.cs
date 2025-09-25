using Core.Logging.Abstractions;
using Serilog;

namespace Core.Logging.Implementation;

public class Logger : Abstractions.ILogger
{
    private readonly Serilog.ILogger _logger;

    public Logger()
    {
        _logger = Log.ForContext<Logger>();
    }

    public void Information(string message, params object[] args) => _logger.Information(message, args);
    public void Warning(string message, params object[] args) => _logger.Warning(message, args);
    public void Error(Exception exception, string message, params object[] args) => _logger.Error(exception, message, args);
    public void Debug(string message, params object[] args) => _logger.Debug(message, args);
    public void Critical(Exception exception, string message, params object[] args) => _logger.Fatal(exception, message, args);
}
