using Serilog;
using Serilog.Events;

namespace Zeng.CoreLibrary.Toolkit.Logging;

internal sealed class TraceInfoTempLogger(ILogger logger) : ILogger
{
    
    private ILogger? BaseLogger { get; set; } = logger;
    
    public void Write(LogEvent logEvent)
    {
        if (BaseLogger is null)
            return;
        BaseLogger.Write(logEvent);
        BaseLogger = null;
    }
}