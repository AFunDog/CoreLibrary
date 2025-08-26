using Serilog.Core;
using Serilog.Events;

namespace Zeng.CoreLibrary.Toolkit.Logging;

/// <summary>
/// <list type="">
/// <item>FilePath - 记录代码文件位置</item>
/// <item>FileName - 记录代码文件名称的扩展名</item>
/// <item>Caller - 记录调用函数名称</item>
/// <item>CallerLine - 记录调用行数</item>
/// <item>TraceInfo - 综合记录信息</item>
/// </list>
/// </summary>
internal sealed class TraceInfoEnricher : ILogEventEnricher
{
    public static TraceInfoEnricher Instance { get; } = new();

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

    public void Reset()
    {
        FilePath = null;
        Caller = null;
        Line = null;
    }
}