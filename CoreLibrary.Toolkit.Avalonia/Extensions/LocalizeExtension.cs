using System.ComponentModel;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Core;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
using Zeng.CoreLibrary.Toolkit.Services.Localization;
using Zeng.CoreLibrary.Toolkit.Structs;

namespace Zeng.CoreLibrary.Toolkit.Avalonia.Extensions;

public sealed class LocalizeExtension
{
    //private HashSet<AvaloniaObject> Objects { get; } = [];

    private object Key { get; } = new();

    public string? StringFormat { get; set; }

    private static LocalizeServiceWrapper Wrapper { get; set; } = new(ILocalizeService.Empty);

    public static void SetLocalizeService(ILocalizeService service)
    {
        Wrapper = new(service);
    }

    public LocalizeExtension() { }

    public LocalizeExtension(object key)
    {
        Key = key;
    }

    public object ProvideValue(IServiceProvider serviceProvider)
    {
        object FromBinding(BindingBase binding)
        {
            var obj = new LocalizationObject(Wrapper.Provider);
            obj.Bind(LocalizationObject.KeyProperty, binding);
            return new CompiledBindingExtension()
            {
                Path = new CompiledBindingPathBuilder()
                    .Property(
                        LocalizationObject.TextProperty,
                        PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor
                    )
                    .Build(),
                Source = obj,
                StringFormat = StringFormat,
            };
        }

        return Key switch
        {
            BindingBase binding => FromBinding(binding),
            string key => new CompiledBindingExtension
            {
                Path = new CompiledBindingPathBuilder()
                    .Property(
                        new ClrPropertyInfo(
                            key,
                            o => (o as LocalizeServiceWrapper)?.Provider.Localize(key),
                            (_, _) => { },
                            typeof(string)
                        ),
                        PropertyInfoAccessorFactory.CreateInpcPropertyAccessor
                    )
                    .Build(),
                Source = Wrapper,
                StringFormat = StringFormat,
            },
            _ => Key,
        };
    }

    private sealed class LocalizationObject : AvaloniaObject
    {
        #region Key

        public static readonly DirectProperty<LocalizationObject, string> KeyProperty = AvaloniaProperty.RegisterDirect<
            LocalizationObject,
            string
        >(nameof(Key), o => o.Key, (o, v) => o.Key = v, unsetValue: string.Empty);

        private string _key = string.Empty;
        public string Key
        {
            get => _key;
            set
            {
                if (SetAndRaise(KeyProperty, ref _key, value))
                    RaisePropertyChanged(TextProperty, string.Empty, LocalizeService.Localize(Key));
            }
        }
        #endregion
        #region Text

        public static readonly DirectProperty<LocalizationObject, string> TextProperty =
            AvaloniaProperty.RegisterDirect<LocalizationObject, string>(
                nameof(Text),
                o => o.Text,
                unsetValue: string.Empty
            );
        public string Text => LocalizeService.Localize(Key);

        #endregion

        private ILocalizeService LocalizeService { get; }

        public LocalizationObject(ILocalizeService provider)
        {
            LocalizeService = provider;

            LocalizeService.LocalizationChanged += OnLocalizationChanged;
        }

        private void OnLocalizationChanged(ILocalizeService sender, LocalizationChangedEventArgs args)
        {
            if (args.Key == Key)
                RaisePropertyChanged(TextProperty, string.Empty, sender.Localize(Key));
        }
    }

    private sealed class LocalizeServiceWrapper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ILocalizeService Provider { get; }

        public LocalizeServiceWrapper(ILocalizeService provider)
        {
            Provider = provider;
            Provider.LocalizationChanged += OnLocalizationChanged;
            //Provider.LocalizeCultureChanged += OnLocalizeCultureChanged;
        }

        //private void OnLocalizeCultureChanged(ILocalizeService service, CultureInfo info) { }

        private void OnLocalizationChanged(ILocalizeService provider, LocalizationChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, new(args.Key));
        }
    }
}
