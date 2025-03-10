using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Toolkit.Structs;

namespace CoreLibrary.Toolkit.Services.Localization;

public interface ILocalizeService
{
    event Action<ILocalizeService, CultureInfo>? LocalizeCultureChanged;
    event Action<ILocalizeService, LocalizationChangedEventArgs>? LocalizationChanged;
    CultureInfo LocalizeCulture { get; set; }
    string Localize(string key);
    string Localize(string key, CultureInfo culture);

    //void SetLocalization(CultureInfo culture, string uid, string value);

    public static ILocalizeService Empty { get; } = new EmptyService();

    sealed class EmptyService : ILocalizeService
    {
        public CultureInfo LocalizeCulture { get; set; }

        public event Action<ILocalizeService, CultureInfo>? LocalizeCultureChanged;
        public event Action<ILocalizeService, LocalizationChangedEventArgs>? LocalizationChanged;

        public string Localize(string key)
        {
            return key;
        }

        public string Localize(string key, CultureInfo culture)
        {
            return key;
        }

        public void SetLocalization(CultureInfo culture, string uid, string value) { }
    }
}
