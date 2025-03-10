using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CoreLibrary.Toolkit.Avalonia.Structs;

/// <summary>
/// <see cref="Controls.NavigationView"/> 的菜单项数据
/// </summary>
public sealed partial class MenuItemData : ObservableObject, IDisposable
{
    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// 图标
    /// </summary>
    public Bitmap? Icon { get; }

    /// <summary>
    /// 字符图标
    /// </summary>
    public char? Glyph { get; }

    /// <summary>
    /// 额外数据
    /// </summary>
    public object? Tag { get; }

    private bool _isSelected;
    internal bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    internal string GlyphString => Glyph.HasValue ? Glyph.Value.ToString() : string.Empty;
    internal bool UseFontIcon => Glyph.HasValue;

    public MenuItemData(string title, Bitmap? icon, object? tag = null)
    {
        Title = title;
        Icon = icon;
        Tag = tag;
    }

    public MenuItemData(string tile, string iconPath, object? tag = null)
        : this(tile, new Bitmap(AssetLoader.Open(new Uri(iconPath))), tag) { }

    public MenuItemData(string title, char glyph, object? tag = null)
        : this(title, icon: null, tag)
    {
        Glyph = glyph;
    }

    public void Dispose()
    {
        Icon?.Dispose();
    }
}
