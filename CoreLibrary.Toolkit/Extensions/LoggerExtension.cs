using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Zeng.CoreLibrary.Core.Contacts;

namespace Zeng.CoreLibrary.Toolkit.Extensions;

public interface ILoggerWrapper : IDisposable, ILogger { }

public static class LoggerExtension
{
    [Obsolete]
    public static ILoggerWrapper LogFunc(
        ILogger logger,
        [CallerMemberName] string caller = "",
        string extendMessage = "",
        bool keepTime = false,
        LogEventLevel timerLevel = LogEventLevel.Debug)
    {
        return new LoggerWrapper(logger, caller, extendMessage, keepTime, timerLevel);
    }

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
        TraceInfoEnricher.Instance.FilePath = filePath;
        TraceInfoEnricher.Instance.Caller = caller;
        TraceInfoEnricher.Instance.Line = line;
        return logger;
    }

    /// <summary>
    /// 添加记录文件、调用者和调用位置信息的 Enricher
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static LoggerConfiguration FromTraceInfo(this LoggerEnrichmentConfiguration configuration)
    {
        return configuration.With(TraceInfoEnricher.Instance);
    }

    private sealed class TraceInfoEnricher : ILogEventEnricher
    {
        public static TraceInfoEnricher Instance { get; } = new();

        public string? FilePath { get; set; }
        public string? Caller { get; set; }
        public int? Line { get; set; }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("FilePath", FilePath));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("Caller", Caller));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("CallerLine", Line));
        }
    }
}

[Obsolete]
internal sealed class LoggerWrapper(
    ILogger logger,
    string caller,
    string? extendMessage,
    bool keepTime,
    LogEventLevel timerLevel) : DisposableObject, ILoggerWrapper
{
    private Stopwatch? Stopwatch { get; } = keepTime ? Stopwatch.StartNew() : null;

    void ILogger.Write(
        LogEventLevel level,
        Exception? exception,
        string messageTemplate,
        params object?[]? propertyValues)
    {
        StringBuilder formatString = new("[{{Caller}}]");
        List<object> args = [caller];

        if (string.IsNullOrEmpty(extendMessage) is false)
        {
            formatString.Append(" {{ExtendMessage}}");
            args.Add(extendMessage);
        }

        if (string.IsNullOrEmpty(messageTemplate) is false)
        {
            formatString.Append(" - {0}");
            logger.Write(
                level,
                exception,
                string.Format(formatString.ToString(), messageTemplate),
                [.. args, .. propertyValues]
            );
        }
        else
        {
            logger.Write(level, exception, string.Format(formatString.ToString()), [.. args, .. propertyValues]);
        }
    }

    public void Write(LogEvent logEvent) { }

    protected override void DisposeManagedResource() { }

    protected override void DisposeUnmanagedResource() { }

    protected override void OnDisposed()
    {
        if (Stopwatch is not null)
        {
            Stopwatch.Stop();
            ((ILogger)this).Write(timerLevel, "执行结束 用时 {Ms}", Stopwatch.ElapsedMilliseconds);
        }
    }
}