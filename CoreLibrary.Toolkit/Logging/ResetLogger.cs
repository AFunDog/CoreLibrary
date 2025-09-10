using Serilog;
using Serilog.Events;

namespace Zeng.CoreLibrary.Toolkit.Logging;

internal sealed class ResetLogger : ILogger
{
    public static ResetLogger Instance { get; } = new();

    public ILogger? BaseLogger { get; set; }

    public void Write(LogEvent logEvent)
    {
        try
        {
            if (BaseLogger is not null)
                BaseLogger.Write(logEvent);
        }
        finally
        {
            BaseLogger = null;
            TraceInfoEnricher.Instance.Reset();
        }
    }
}