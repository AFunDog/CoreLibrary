using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Toolkit.Services.Localization;

namespace CoreServicesWinUILibrary.Extensions
{
    public static class LocalizeExtension
    {
        public static string Localize(this string value)
        {
            return App.GetService<ILocalizeService>().Localize(value);
        }
    }
}
