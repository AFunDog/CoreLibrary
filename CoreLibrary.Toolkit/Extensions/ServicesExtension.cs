using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Toolkit.Services.Localization;
using CoreLibrary.Toolkit.Services.Navigate;
using Microsoft.Extensions.DependencyInjection;

namespace CoreLibrary.Toolkit.Extensions;

public static class ServicesExtension
{
    private static FrozenDictionary<Type, Type> ServiceTypeSet { get; } =
        new Dictionary<Type, Type>
        {
            [typeof(INavigateService)] = typeof(NavigateService),
            [typeof(ILocalizeService)] = typeof(LocalizeService),
        }.ToFrozenDictionary();

    public static IServiceCollection UseCoreServiceSingleton<T>(this IServiceCollection services)
        where T : notnull
    {
        if (ServiceTypeSet.TryGetValue(typeof(T), out var implType))
        {
            services.AddSingleton(typeof(T), implType);
        }
        return services;
    }

    public static IServiceCollection UseCoreServiceSingleton<T>(this IServiceCollection services, object? key)
        where T : notnull
    {
        if (ServiceTypeSet.TryGetValue(typeof(T), out var implType))
        {
            services.AddKeyedSingleton(typeof(T), key, implType);
        }
        return services;
    }

    public static IServiceCollection UseCoreServiceScoped<T>(this IServiceCollection services)
        where T : notnull
    {
        if (ServiceTypeSet.TryGetValue(typeof(T), out var implType))
        {
            services.AddScoped(typeof(T), implType);
        }
        return services;
    }

    public static IServiceCollection UseCoreServiceTransient<T>(this IServiceCollection services)
        where T : notnull
    {
        if (ServiceTypeSet.TryGetValue(typeof(T), out var implType))
        {
            services.AddTransient(typeof(T), implType);
        }
        return services;
    }
}
