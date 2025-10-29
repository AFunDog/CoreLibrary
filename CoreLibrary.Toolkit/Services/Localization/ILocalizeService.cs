using System.Globalization;
using Zeng.CoreLibrary.Toolkit.Structs;

namespace Zeng.CoreLibrary.Toolkit.Services.Localization;

/// <summary>
/// 本地化服务
/// 将对应的key翻译成指定语言的文本
/// </summary>
public interface ILocalizeService
{
    event Action<ILocalizeService, CultureInfo>? LocalizeCultureChanged;
    event Action<ILocalizeService, LocalizationChangedEventArgs>? LocalizationChanged;
    CultureInfo LocalizeCulture { get; set; }
    string Localize(string key,string? fallback = null);
    string Localize(string key, CultureInfo culture,string? fallback = null);

    //void SetLocalization(CultureInfo culture, string uid, string value);

    public static ILocalizeService Empty { get; } = new EmptyService();

    private sealed class EmptyService : ILocalizeService
    {
        public CultureInfo LocalizeCulture { get; set; } = CultureInfo.CurrentCulture;

        public event Action<ILocalizeService, CultureInfo>? LocalizeCultureChanged;
        public event Action<ILocalizeService, LocalizationChangedEventArgs>? LocalizationChanged;

        public string Localize(string key,string? fallback = null) => key;

        public string Localize(string key, CultureInfo culture,string? fallback = null) => key;

        public void SetLocalization(CultureInfo culture, string uid, string value) { }
    }
}
