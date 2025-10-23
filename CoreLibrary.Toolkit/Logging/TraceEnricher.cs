using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Zeng.CoreLibrary.Toolkit.Logging;

internal sealed class TraceEnricher : ILogEventEnricher
{
    public string? FilePath { get; set; }
    public string? Caller { get; set; }
    public int? Line { get; set; }

    // TODO 当 SourceContext 为空时，使用 FilePath 作为 SourceContext
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (FilePath is not null)
        {
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("FilePath", FilePath));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("FileName", Path.GetFileName(FilePath)));
        }

        if (Caller is not null)
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("Caller", Caller));
        if (Line is not null)
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("CallerLine", Line));

        // if (FilePath is not null && Caller is not null && Line is not null)
        //     logEvent.AddOrUpdateProperty(
        //         propertyFactory.CreateProperty("TraceInfo", $"{Path.GetFileName(FilePath)}>{Caller}({Line}) ")
        //     );
    }
}