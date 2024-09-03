using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Toolkit.Services.Localization
{
    public interface ILocalizeService
    {
        event Action<ILocalizeService, CultureInfo>? CurrentCultureChanged;

        CultureInfo CurrentCultrue { get; set; }

        string Localize(string uid);
        string Localize(string uid, CultureInfo culture);

        void SetLocalization(CultureInfo culture, string uid, string value);
    }
}
