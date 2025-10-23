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
        = "[{Timestamp:HH:mm:ss fff} {Level:u3}] {TraceInfo}{Message:lj}{NewLine}{Exception}";

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