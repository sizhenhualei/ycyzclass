using Microsoft.Extensions.Logging;

namespace YcyzClass.Services.Logging;

public class SentryLoggerProvider : ILoggerProvider
{
    public void Dispose()
    {
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new SentryEventLogger(categoryName);
    }
}