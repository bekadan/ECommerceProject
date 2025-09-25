namespace Core.Logging.Abstractions;

public interface ILogger
{
    void Information(string message, params object[] args);
    void Warning(string message, params object[] args);
    void Error(Exception exception, string message, params object[] args);
    void Debug(string message, params object[] args);
    void Critical(Exception exception, string message, params object[] args);
}