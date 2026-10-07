using Avalonia;
using Avalonia.Logging;
using YcyzClass.Services.Logging;

namespace YcyzClass.Extensions;

public static class AvaloniaLoggingSinkExtensions
{
    public static AppBuilder LogToHostSink(this AppBuilder builder, LogEventLevel level = LogEventLevel.Warning)
    {
        Logger.Sink = new AvaloniaLoggingSink(level);
        return builder;
    }
}