using CoreServices.Template;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoreServices.Localization
{
    public sealed class LocalizeService : DisposableTemplate, ILocalizeService
    {
        private readonly Dictionary<CultureInfo, Dictionary<string, string>> _localizations = [];
        private Action? _localizeActions;

        private CultureInfo _currentCulture = CultureInfo.CurrentCulture;
        public CultureInfo CurrentCultrue
        {
            get => _currentCulture;
            set
            {
                if (_currentCulture != value)
                {
                    _currentCulture = value;
                    CurrentCultureChanged?.Invoke(this, _currentCulture);
                }
            }
        }
        public event Action<ILocalizeService, CultureInfo>? CurrentCultureChanged;

        public string Localize(string uid)
        {
            return Localize(uid, CurrentCultrue);
        }
        public string Localize(string uid, CultureInfo culture)
        {
            if (_localizations.TryGetValue(culture, out var loc))
            {
                if (loc.TryGetValue(uid, out var value))
                {
                    return value;
                }
            }
            return uid;
        }
        public void SetLocalization(CultureInfo culture, string uid, string value)
        {
            if (_localizations.TryGetValue(culture, out var loc))
            {
                loc[uid] = value;
            }
            else
            {
                _localizations[culture] = new()
                {
                    [uid] = value,
                };
            }
        }

        protected override void DestoryManagedResource()
        {
            _localizations.Clear();
            _localizeActions = null;
        }

        protected override void DestoryUnmanagedResource()
        {

        }
    }
}
