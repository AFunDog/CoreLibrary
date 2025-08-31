using Microsoft.Extensions.DependencyInjection;
using Zeng.CoreLibrary.Toolkit.Windows.Services.SystemMonitor;
using Zeng.CoreLibrary.Toolkit.Windows.Services.SystemMonitor.Internals;

namespace Zeng.CoreLibrary.Toolkit.Windows.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection UseSystemMonitor(this IServiceCollection services)
    {
        services.AddSingleton<IPerformanceMonitor, PerformanceMonitor>();
        return services;
    }
}