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

    private CultureInfo _currentCulture = CultureInfo.CurrentCulture;

    public CultureInfo LocalizeCulture
    {
        get => _currentCulture;
        set
        {
            if (Equals(_currentCulture, value))
                return;

            // 当服务切换翻译语言时，通知改变

            HashSet<string> notifyChangedKeySet = [];
            if (LocalizationTable.TryGetValue(_currentCulture, out var dict))
            {
                foreach (var key in dict.Keys)
                {
                    notifyChangedKeySet.Add(key);
                }
            }

            _currentCulture = value;
            LocalizeCultureChanged?.Invoke(this, _currentCulture);

            if (LocalizationTable.TryGetValue(_currentCulture, out dict))
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
    }

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