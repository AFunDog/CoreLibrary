using System;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Zeng.CoreLibrary.Toolkit.Extensions;
using Zeng.CoreLibrary.Toolkit.Logging;
using Zeng.CoreLibrary.Toolkit.Windows.Extensions;


namespace Zeng.CoreLibrary.Toolkit.Avalonia.TestApp;

sealed class Program
{
    public static IServiceProvider ServiceProvider { get; } = new ServiceCollection()
        .AddSingleton<ILogger>(_
            => new LoggerConfiguration()
                .Enrich.FromTraceInfo()
                .MinimumLevel.Verbose()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext.Short}{FileName}>{Caller}({CallerLine}) {Message:lj}{NewLine}{Exception}")
                .CreateLogger()
        )
        .UseSystemMonitor()
        .BuildServiceProvider();

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        ServiceProvider.TryDispose();
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>().UsePlatformDetect().WithInterFont().LogToTrace();
}