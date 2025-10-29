using System.Globalization;
using Serilog;
using Zeng.CoreLibrary.Core.Contacts;
using Zeng.CoreLibrary.Toolkit.Contacts;
using Zeng.CoreLibrary.Toolkit.Logging;
using Zeng.CoreLibrary.Toolkit.Structs;

namespace Zeng.CoreLibrary.Toolkit.Services.Localization;

internal sealed class LocalizeService : DisposableObject, ILocalizeService
{
    private Dictionary<CultureInfo, Dictionary<string, string>> LocalizationTable { get; } = [];

    private ILogger Logger { get; }

    private IDataProvider<IEnumerable<LocalizationData>>[] DataProviders { get; }

    // 默认语言，当其他语言不存在或翻译不存在时尝试使用默认翻译
    private CultureInfo DefaultCulture { get; } = CultureInfo.GetCultureInfo(string.Empty); 
    
    public CultureInfo LocalizeCulture
    {
        get;
        set
        {
            if (Equals(field, value))
                return;

            // 当服务切换翻译语言时，通知改变

            HashSet<string> notifyChangedKeySet = [];
            if (LocalizationTable.TryGetValue(field, out var dict))
            {
                foreach (var key in dict.Keys)
                {
                    notifyChangedKeySet.Add(key);
                }
            }

            field = value;
            LocalizeCultureChanged?.Invoke(this, field);

            if (LocalizationTable.TryGetValue(field, out dict))
            {
                foreach (var key in dict.Keys)
                {
                    notifyChangedKeySet.Add(key);
                }
            }

            foreach (var key in notifyChangedKeySet)
            {
                LocalizationChanged?.Invoke(this, new(key));
            }
        }
    } = CultureInfo.CurrentCulture;

    public event Action<ILocalizeService, CultureInfo>? LocalizeCultureChanged;
    public event Action<ILocalizeService, LocalizationChangedEventArgs>? LocalizationChanged;

    public LocalizeService() : this(Log.Logger) { }

    public LocalizeService(ILogger logger) : this(logger, []) { }

    public LocalizeService(ILogger logger, IEnumerable<IDataProvider<IEnumerable<LocalizationData>>> dataProviders)
    {
        Logger = logger.ForContext<LocalizeService>();
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

    private void OnDataProviderDataChanged(
        IDataProvider<IEnumerable<LocalizationData>> provider,
        DataProviderDataChangedEventArgs<IEnumerable<LocalizationData>> providerDataChangedEventArgs)
    {
        LoadLocalization();
    }

    public string Localize(string key,string? fallback = null)
    {
        return Localize(key, LocalizeCulture,fallback);
    }

    public string Localize(string key, CultureInfo culture,string? fallback = null)
    {
        if (LocalizationTable.TryGetValue(culture, out var loc))
        {
            if (loc.TryGetValue(key, out var value))
            {
                return value;
            }
        }

        return LocalizationTable.GetValueOrDefault(DefaultCulture)?.GetValueOrDefault(key) ?? fallback ?? key;
    }

    private void LoadLocalization()
    {
        foreach (var dataProvider in DataProviders)
        {
            foreach (var data in dataProvider.Data ?? [])
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
        Logger.Trace().Verbose("销毁");
    }
}