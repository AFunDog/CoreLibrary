using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Zeng.CoreLibrary.Toolkit.Avalonia.Structs;

/// <summary>
/// <see cref="Controls.NavigationView"/> 的菜单项数据
/// </summary>
public sealed partial class MenuItemData : ObservableObject
{
    /// <summary>
    /// 标题
    /// </summary>
    [ObservableProperty]
    public partial string Title { get; set; } = string.Empty;
    
    [ObservableProperty]
    public partial object? Icon { get; set; }

    // /// <summary>
    // /// 图标
    // /// </summary>
    // [ObservableProperty]
    // public partial Bitmap? Icon { get; set; }
    //
    // /// <summary>
    // /// 字符图标
    // /// </summary>
    // [ObservableProperty]
    // public partial char? Glyph
    // {
    //     get;
    //     set
    //     {
    //         if (SetProperty(ref field, value))
    //         {
    //             OnPropertyChanged(nameof(GlyphString));
    //             OnPropertyChanged(nameof(UseFontIcon));
    //         }
    //     }
    // }

    /// <summary>
    /// 额外数据
    /// </summary>
    [ObservableProperty]
    public partial object? Tag { get; set; }

    [ObservableProperty]
    internal partial bool IsSelected { get; set; }

    // internal string GlyphString => Glyph.HasValue ? Glyph.Value.ToString() : string.Empty;
    // internal bool UseFontIcon => Glyph.HasValue;

    public MenuItemData(string title, object? icon, object? tag = null)
    {
        Title = title;
        Icon = icon;
        Tag = tag;
    }
}