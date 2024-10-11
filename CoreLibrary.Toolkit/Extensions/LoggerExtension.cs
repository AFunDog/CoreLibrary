using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Core.BasicObjects;
using Serilog;
using Serilog.Events;

namespace CoreLibrary.Toolkit.Extensions
{
    public interface ILoggerWrapper : IDisposable, ILogger { }

    public static class LoggerExtension
    {
        public static ILoggerWrapper LogFunc(
            ILogger logger,
            [CallerMemberName] string caller = "",
            string extendMessage = "",
            bool keepTime = false,
            LogEventLevel timerLevel = LogEventLevel.Debug
        )
        {
            return new LoggerWrapper(logger, caller, extendMessage, keepTime, timerLevel);
        }
    }

    internal sealed class LoggerWrapper(
        ILogger logger,
        string caller,
        string? extendMessage,
        bool keepTime,
        LogEventLevel timerLevel
    ) : DisposableObject, ILoggerWrapper
    {
        private Stopwatch? Stopwatch { get; } = keepTime ? Stopwatch.StartNew() : null;

        void ILogger.Write(
            LogEventLevel level,
            Exception? exception,
            string messageTemplate,
            params object?[]? propertyValues
        )
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
}
