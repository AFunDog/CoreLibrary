using Microsoft.Extensions.DependencyInjection;
using Zeng.CoreLibrary.Toolkit.Services.Localization;
using Zeng.CoreLibrary.Toolkit.Services.Navigate;

namespace Zeng.CoreLibrary.Toolkit.Extensions;

using LocalizeServiceImpl = LocalizeService;
using NavigateServiceImpl = NavigateService;

/// <summary>
/// 注册 CoreLibrary.Toolkit 服务扩展
/// </summary>
public static class ServicesExtension
{
    /// <summary>
    /// 使用导航服务
    /// </summary>
    public static IServiceCollection UseNavigateService(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton
    )
    {
        return lifetime switch
        {
            ServiceLifetime.Singleton => services.AddSingleton<INavigateService, NavigateServiceImpl>(),
            ServiceLifetime.Scoped => services.AddScoped<INavigateService, NavigateServiceImpl>(),
            ServiceLifetime.Transient => services.AddTransient<INavigateService, NavigateServiceImpl>(),
            _ => services,
        };
    }

    /// <summary>
    /// 使用本地化服务
    /// </summary>
    public static IServiceCollection UseLocalizeService(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton
    )
    {
        return lifetime switch
        {
            ServiceLifetime.Singleton => services.AddSingleton<ILocalizeService, LocalizeServiceImpl>(),
            ServiceLifetime.Scoped => services.AddScoped<ILocalizeService, LocalizeServiceImpl>(),
            ServiceLifetime.Transient => services.AddTransient<ILocalizeService, LocalizeServiceImpl>(),
            _ => services,
        };
    }

    // 以下方式不支持 AOT

    //private static FrozenDictionary<Type, Type> ServiceTypeSet { get; } =
    //    new Dictionary<Type, Type>
    //    {
    //        [typeof(INavigateService)] = typeof(NavigateService),
    //        [typeof(ILocalizeService)] = typeof(LocalizeService),
    //    }.ToFrozenDictionary();

    //public static IServiceCollection UseCoreServiceSingleton<T>(this IServiceCollection services)
    //    where T : notnull
    //{
    //    if (ServiceTypeSet.TryGetValue(typeof(T), out var implType))
    //    {
    //        services.AddSingleton(typeof(T), implType);
    //    }
    //    return services;
    //}

    //public static IServiceCollection UseCoreServiceSingleton<T>(this IServiceCollection services, object? key)
    //    where T : notnull
    //{
    //    if (ServiceTypeSet.TryGetValue(typeof(T), out var implType))
    //    {
    //        services.AddKeyedSingleton(typeof(T), key, implType);
    //    }
    //    return services;
    //}

    //public static IServiceCollection UseCoreServiceScoped<T>(this IServiceCollection services)
    //    where T : notnull
    //{
    //    if (ServiceTypeSet.TryGetValue(typeof(T), out var implType))
    //    {
    //        services.AddScoped(typeof(T), implType);
    //    }
    //    return services;
    //}

    //public static IServiceCollection UseCoreServiceTransient<T>(this IServiceCollection services)
    //    where T : notnull
    //{
    //    if (ServiceTypeSet.TryGetValue(typeof(T), out var implType))
    //    {
    //        services.AddTransient(typeof(T), implType);
    //    }
    //    return services;
    //}
}
