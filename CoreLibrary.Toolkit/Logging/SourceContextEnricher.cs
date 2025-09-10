using Serilog.Core;
using Serilog.Events;

namespace Zeng.CoreLibrary.Toolkit.Logging;



/// <summary>
/// 基于 SourceContext 的日志富集器
/// <br/>
/// 默认情况下，SourceContext 的值是完全类名，包含命名空间，例如：Zeng.CoreLibrary.Toolkit.Logging.SourceContextEnricher
/// <br/>
/// 但此富集器会基于 SourceContext 的值提供 SourceContext.Short 属性，只包含类型名称，例如 SourceContextEnricher
/// </summary>
/// <remarks>
/// 同时会在属性后加上一个默认空格，这样在没有记录 SourceContext 的时候，不会产生空格
/// </remarks>
internal class SourceContextEnricher : ILogEventEnricher
{
    public static SourceContextEnricher Instance { get; } = new();
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logEvent"></param>
    /// <param name="factory"></param>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory factory)
    {
        if (logEvent.Properties.TryGetValue("SourceContext", out var value)
            && value is ScalarValue { Value: string name })
        {
            var shortName = name[(name.LastIndexOf('.') + 1)..];
            logEvent.AddOrUpdateProperty(factory.CreateProperty("SourceContext", $"{name} "));
            logEvent.AddOrUpdateProperty(factory.CreateProperty("SourceContext.Short", $"{shortName} "));
        }
    }
}