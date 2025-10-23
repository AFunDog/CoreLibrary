using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Zeng.CoreLibrary.Core.Contacts;

namespace Zeng.CoreLibrary.Toolkit.Logging;

// public interface ILoggerWrapper : IDisposable, ILogger { }

public static partial class LoggerExtension
{
    /// <summary>
    /// 控制台用的消息模板
    /// </summary>
    public const string ConsoleMessageTemplate
        = "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext.Short}{FileName}>{Caller}({CallerLine}) {Message:lj}{NewLine}{Exception}";

    /// <summary>
    /// 额外记录文件、调用者和调用位置的信息
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="filePath"></param>
    /// <param name="caller"></param>
    /// <param name="line"></param>
    /// <returns></returns>
    public static ILogger Trace(
        this ILogger logger,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string caller = "",
        [CallerLineNumber] int line = 0)
    {
        // BUG 存在线程安全问题，可能导致信息错误
        // TraceInfoEnricher.Instance.FilePath = filePath;
        // TraceInfoEnricher.Instance.Caller = caller;
        // TraceInfoEnricher.Instance.Line = line;
        // ResetLogger.Instance.BaseLogger = logger;
        // return ResetLogger.Instance;
        return logger.ForContext(new TraceEnricher() { FilePath = filePath, Caller = caller, Line = line });
    }

    /// <summary>
    /// 整合记录文件、调用者和调用位置信息的 Enricher
    /// <inheritdoc cref="TraceInfoEnricher"/>
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static LoggerConfiguration FromTraceInfo(this LoggerEnrichmentConfiguration configuration)
    {
        return configuration.With(TraceInfoEnricher.Instance);
    }

    /// <summary>
    /// <inheritdoc cref="SourceContextEnricher"/>
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static LoggerConfiguration ShortSourceContext(this LoggerEnrichmentConfiguration configuration)
    {
        return configuration.With(SourceContextEnricher.Instance);
    }
}

// [Obsolete]
// internal sealed class LoggerWrapper(
//     ILogger logger,
//     string caller,
//     string? extendMessage,
//     bool keepTime,
//     LogEventLevel timerLevel) : DisposableObject, ILoggerWrapper
// {
//     private Stopwatch? Stopwatch { get; } = keepTime ? Stopwatch.StartNew() : null;
//
//     void ILogger.Write(
//         LogEventLevel level,
//         Exception? exception,
//         string messageTemplate,
//         params object?[]? propertyValues)
//     {
//         StringBuilder formatString = new("[{{Caller}}]");
//         List<object> args = [caller];
//
//         if (string.IsNullOrEmpty(extendMessage) is false)
//         {
//             formatString.Append(" {{ExtendMessage}}");
//             args.Add(extendMessage);
//         }
//
//         if (string.IsNullOrEmpty(messageTemplate) is false)
//         {
//             formatString.Append(" - {0}");
//             logger.Write(
//                 level,
//                 exception,
//                 string.Format(formatString.ToString(), messageTemplate),
//                 [.. args, .. propertyValues]
//             );
//         }
//         else
//         {
//             logger.Write(level, exception, string.Format(formatString.ToString()), [.. args, .. propertyValues]);
//         }
//     }
//
//     public void Write(LogEvent logEvent) { }
//
//     protected override void DisposeManagedResource() { }
//
//     protected override void DisposeUnmanagedResource() { }
//
//     protected override void OnDisposed()
//     {
//         if (Stopwatch is not null)
//         {
//             Stopwatch.Stop();
//             ((ILogger)this).Write(timerLevel, "执行结束 用时 {Ms}", Stopwatch.ElapsedMilliseconds);
//         }
//     }
// }