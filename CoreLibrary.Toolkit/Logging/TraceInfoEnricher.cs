using System.Text;
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

    // public string? FilePath { get; set; }
    // public string? Caller { get; set; }
    // public int? Line { get; set; }

    // TODO 当 SourceContext 为空时，使用 FilePath 作为 SourceContext
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (GetSpace() is { } spaceValue)
        {
            var builder = new StringBuilder().Append(spaceValue);
            if (logEvent.Properties.TryGetValue("Caller", out var caller)
                && caller is ScalarValue { Value: { } callerValue })
            {
                builder.Append('|').Append(callerValue);
            }
            
            if (logEvent.Properties.TryGetValue("CallerLine", out var callerLine)
                && callerLine is ScalarValue { Value: { } callerLineValue })
            {
                builder.Append('(').Append(callerLineValue).Append(')');
            }
            


            builder.Append(' ');
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("TraceInfo", builder.ToString()));
        }

        string? GetSpace()
        {
            if (logEvent.Properties.TryGetValue("FilePath", out var filePath)
                && filePath is ScalarValue { Value : { } filePathValue })
            {
                return Path.GetFileNameWithoutExtension(filePathValue.ToString());
            }

            if (logEvent.Properties.TryGetValue("SourceContext.Short", out var sourceContextShort)
                && sourceContextShort is ScalarValue { Value : { } sourceContextShortValue })
            {
                return sourceContextShortValue.ToString()?.Trim();
            }

            return null;
        }

    }
}