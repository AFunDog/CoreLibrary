using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CoreLibrary.Core.Contacts;
using CoreLibrary.Toolkit.Avalonia.Structs;
using CoreLibrary.Toolkit.Contacts;
using CoreLibrary.Toolkit.Structs;
using Serilog;

namespace CoreLibrary.Toolkit.Services.Localization;

// TODO 改为可以接收多个本地化数据提供者

internal sealed class LocalizeService : DisposableObject, ILocalizeService
{
    private Dictionary<CultureInfo, Dictionary<string, string>> LocalizationTable { get; } = [];

    private ILogger Logger { get; }

    private IDataProvider<LocalizationData>[] DataProviders { get; }

    private CultureInfo _currentCulture = CultureInfo.CurrentCulture;
    public CultureInfo LocalizeCulture
    {
        get => _currentCulture;
        set
        {
            if (_currentCulture != value)
            {
                HashSet<string> notifys = [];
                if (LocalizationTable.TryGetValue(_currentCulture, out var dict))
                {
                    foreach (var key in dict.Keys)
                    {
                        notifys.Add(key);
                    }
                }
                _currentCulture = value;
                LocalizeCultureChanged?.Invoke(this, _currentCulture);
                if (LocalizationTable.TryGetValue(_currentCulture, out dict))
                {
                    foreach (var key in dict.Keys)
                    {
                        notifys.Add(key);
                    }
                }
                foreach (var key in notifys)
                {
                    LocalizationChanged?.Invoke(this, new(key));
                }
            }
        }
    }

    public event Action<ILocalizeService, CultureInfo>? LocalizeCultureChanged;
    public event Action<ILocalizeService, LocalizationChangedEventArgs>? LocalizationChanged;

    public LocalizeService()
        : this(Log.Logger) { }

    public LocalizeService(ILogger logger)
        : this(logger, []) { }

    public LocalizeService(ILogger logger, IEnumerable<IDataProvider<LocalizationData>> dataProviders)
    {
        Logger = logger;
        DataProviders = [.. dataProviders];

        foreach (var dataProvider in DataProviders)
        {
            dataProvider.LoadData();
        }
        LoadLocalization();

        foreach (var dataProvider in DataProviders)
        {
            dataProvider.DataChanged += OnDataProviderDataChanged;
        }

        //DataProviders.DataChanged += OnDataProviderDataChanged;
    }

    private void OnDataProviderDataChanged(IDataProvider<LocalizationData> provider)
    {
        LoadLocalization();
    }

    public string Localize(string key)
    {
        return Localize(key, LocalizeCulture);
    }

    public string Localize(string key, CultureInfo culture)
    {
        if (LocalizationTable.TryGetValue(culture, out var loc))
        {
            if (loc.TryGetValue(key, out var value))
            {
                return value;
            }
        }
        return key;
    }

    private void LoadLocalization()
    {
        foreach (var dataProvider in DataProviders)
        {
            foreach (var data in dataProvider.Datas)
            {
                if (LocalizationTable.TryGetValue(data.CultureInfo, out var dict))
                {
                    dict[data.Key] = data.Value;
                }
                else
                {
                    LocalizationTable[data.CultureInfo] = new() { [data.Key] = data.Value };
                }
            }
        }
    }

    protected override void DisposeManagedResource()
    {
        LocalizationTable.Clear();
    }

    protected override void DisposeUnmanagedResource() { }

    protected override void OnDisposed()
    {
        Logger.Verbose("{Service} 已被销毁", nameof(LocalizeService));
    }
}
