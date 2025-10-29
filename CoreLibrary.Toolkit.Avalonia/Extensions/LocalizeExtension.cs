using System.ComponentModel;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Core;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
using Avalonia.Metadata;
using Zeng.CoreLibrary.Toolkit.Services.Localization;
using Zeng.CoreLibrary.Toolkit.Structs;

namespace Zeng.CoreLibrary.Toolkit.Avalonia.Extensions;

/// <summary>
/// 本地化扩展
/// 主要在 XAML 中使用，用来对 UI 元素进行本地化
/// </summary>
public sealed class LocalizeExtension
{
    //private HashSet<AvaloniaObject> Objects { get; } = [];

    [Content]
    public object Key { get; set; } = new();

    public string? Fallback { get; set; } = null;

    /// <summary>
    /// 格式化字符串
    /// </summary>
    public string? StringFormat { get; set; }

    private static LocalizeServiceWrapper Wrapper { get; set; } = new(ILocalizeService.Empty);

    /// <summary>
    /// 用来设置本地化扩展使用本地化服务
    /// </summary>
    /// <param name="service"></param>
    public static void SetLocalizeService(ILocalizeService service)
    {
        Wrapper = new(service);
    }

    public LocalizeExtension() { }

    public LocalizeExtension(object key)
    {
        Key = key;
    }

    /// <summary>
    /// 在 XAML 中自动调用的函数
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public object ProvideValue(IServiceProvider serviceProvider)
    {
        return Key switch
        {
            IBinding binding => FromBinding(binding),
            string key => new CompiledBindingExtension
            {
                Path = new CompiledBindingPathBuilder()
                    .Property(
                        new ClrPropertyInfo(
                            key,
                            o => (o as LocalizeServiceWrapper)?.Provider.Localize(key, Fallback),
                            (_, _) => { },
                            typeof(string)
                        ),
                        PropertyInfoAccessorFactory.CreateInpcPropertyAccessor
                    )
                    .Build(),
                Source = Wrapper,
                StringFormat = StringFormat,
            },
            _ => Fallback ?? Key,
        };

        object FromBinding(IBinding binding)
        {
            // 使用 LocalizationObject 作为中间对象
            // 将传入的绑定表达式 BindingBase 绑定到 LocalizationObject 的 Key 属性
            // 并将 LocalizationObject 的 Text 属性导出
            var obj = new LocalizationObject(Wrapper.Provider) { Fallback = Fallback };
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
    }

    /// <summary>
    /// 用来作为 LocalizeExtension 的中间对象，实现当本地化文本或语言发生变化时，翻译文本也发生变化
    /// </summary>
    private sealed class LocalizationObject : AvaloniaObject
    {
        #region Key

        public static readonly DirectProperty<LocalizationObject, string> KeyProperty
            = AvaloniaProperty.RegisterDirect<LocalizationObject, string>(
                nameof(Key),
                o => o.Key,
                (o, v) => o.Key = v,
                unsetValue: string.Empty
            );

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

        public static readonly DirectProperty<LocalizationObject, string> TextProperty
            = AvaloniaProperty.RegisterDirect<LocalizationObject, string>(
                nameof(Text),
                o => o.Text,
                unsetValue: string.Empty
            );

        public string Text => LocalizeService.Localize(Key);

        #endregion

        public string? Fallback { get; set; }

        private ILocalizeService LocalizeService { get; }

        public LocalizationObject(ILocalizeService provider)
        {
            LocalizeService = provider;

            LocalizeService.LocalizationChanged += OnLocalizationChanged;
        }

        private void OnLocalizationChanged(ILocalizeService sender, LocalizationChangedEventArgs args)
        {
            if (args.Key == Key)
                RaisePropertyChanged(TextProperty, string.Empty, sender.Localize(Key, Fallback));
        }
    }

    /// <summary>
    /// 用来将 ILocalizeService 的 LocalizationChanged 事件转换成 PropertyChanged 事件
    /// </summary>
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