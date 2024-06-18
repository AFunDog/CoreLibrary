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
    public sealed class LocalizeService : DisposableTemplate
    {
        private readonly Dictionary<CultureInfo, Dictionary<string, string>> _localizations = [];
        private readonly Dictionary<string, List<KeyValuePair<object, PropertyInfo>>> _bindActions = [];
        private Action? _localizeActions;
        private CultureInfo _locCulture;
        public CultureInfo LocCulture
        {
            get => _locCulture;
            set
            {
                if (_locCulture != value)
                {
                    _locCulture = value;
                    ExecuteBindActions();
                    ExecuteLocalizeActions();
                    LocCultureChanged?.Invoke(_locCulture);
                }
            }
        }
        public event Action<CultureInfo>? LocCultureChanged;

        public LocalizeService(CultureInfo? culture = null)
        {
            _locCulture = culture ?? CultureInfo.CurrentCulture;
        }
        public string Localize(string uid)
        {
            return Localize(LocCulture, uid);
        }
        public string Localize(CultureInfo culture, string uid)
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
        public void SetLoc(CultureInfo culture, string uid, string value)
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
        public void BindLocalize(object target, PropertyInfo property, string uid)
        {
            if (_bindActions.TryGetValue(uid, out var binds))
            {
                binds.Add(new(target, property));
            }
            else
            {
                _bindActions[uid] = [new(target, property)];
            }
            property.SetValue(target, Localize(LocCulture, uid));
        }
        public void SetLocalizeAction(Action action)
        {
            if (_localizeActions is null)
            {
                _localizeActions = action;
            }
            else
            {
                _localizeActions += action;
            }
        }

        private void ExecuteBindActions()
        {
            foreach ((var uid, var binds) in _bindActions)
            {
                foreach ((var target, var property) in binds)
                {
                    property.SetValue(target, Localize(LocCulture, uid));
                }
            }


        }
        private void ExecuteLocalizeActions()
        {
            _localizeActions?.Invoke();
        }



        protected override void DestoryManagedResource()
        {
            _localizations.Clear();
            _bindActions.Clear();
            _localizeActions = null;
        }

        protected override void DestoryUnmanagedResource()
        {

        }
    }
}
