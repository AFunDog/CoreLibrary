using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;
using CommunityToolkit.Mvvm.Input;
using Zeng.CoreLibrary.Toolkit.Avalonia.Structs;

namespace Zeng.CoreLibrary.Toolkit.Avalonia.Controls;

/// <summary>
/// 导航视图控件，附带侧边的菜单面板
/// </summary>
[TemplatePart("PART_ViewPresenter", typeof(ContentControl))]
[TemplatePart("PART_MenuPanel", typeof(Control))]
public sealed class NavigationView : TemplatedControl
{
    #region IsMenuOpen

    /// <summary>
    /// <inheritdoc cref="IsMenuOpen"/>
    /// </summary>
    public static readonly StyledProperty<bool> IsMenuOpenProperty = AvaloniaProperty.Register<NavigationView, bool>(
        nameof(IsMenuOpen)
    );

    /// <summary>
    /// 菜单面板是否打开
    /// </summary>
    public bool IsMenuOpen
    {
        get => GetValue(IsMenuOpenProperty);
        set => SetValue(IsMenuOpenProperty, value);
    }
    #endregion
    #region MenuPanelExpandedWidth

    /// <summary>
    /// <inheritdoc cref="MenuPanelExpandedWidth"/>
    /// </summary>
    public static readonly StyledProperty<double> MenuPanelExpandedWidthProperty = AvaloniaProperty.Register<
        NavigationView,
        double
    >(nameof(MenuPanelExpandedWidth), 196);

    /// <summary>
    /// 菜单面板扩展宽度
    /// </summary>
    public double MenuPanelExpandedWidth
    {
        get => GetValue(MenuPanelExpandedWidthProperty);
        set => SetValue(MenuPanelExpandedWidthProperty, value);
    }

    #endregion
    #region MenuItemSource

    /// <summary>
    /// <inheritdoc cref="MenuItemSource"/>
    /// </summary>
    public static readonly StyledProperty<IEnumerable<MenuItemData>?> MenuItemSourceProperty =
        AvaloniaProperty.Register<NavigationView, IEnumerable<MenuItemData>?>(nameof(MenuItemSource));

    /// <summary>
    /// 菜单面板的菜单项的数据来源
    /// </summary>
    public IEnumerable<MenuItemData>? MenuItemSource
    {
        get => GetValue(MenuItemSourceProperty);
        set => SetValue(MenuItemSourceProperty, value);
    }

    #endregion
    #region Command

    /// <summary>
    /// <inheritdoc cref="Command"/>
    /// </summary>
    public static readonly StyledProperty<ICommand?> CommandProperty = AvaloniaProperty.Register<
        NavigationView,
        ICommand?
    >(nameof(Command));

    /// <summary>
    /// 菜单项被选中触发的命令
    /// </summary>
    /// <remarks>
    /// 参数为 <see cref="MenuItemData"/>
    /// </remarks>
    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
    #endregion
    #region Content

    /// <summary>
    /// <inheritdoc cref="Content"/>
    /// </summary>
    public static readonly StyledProperty<object?> ContentProperty =
        ContentPresenter.ContentProperty.AddOwner<NavigationView>();

    /// <summary>
    /// 控件展示页面内容
    /// </summary>
    [Content]
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }
    #endregion
    #region MenuTopButtonCommand

    /// <summary>
    /// <inheritdoc cref="MenuTopButtonCommand"/>
    /// </summary>
    internal static readonly DirectProperty<NavigationView, ICommand> MenuTopButtonCommandProperty =
        AvaloniaProperty.RegisterDirect<NavigationView, ICommand>(
            nameof(MenuTopButtonCommand),
            o => o.MenuTopButtonCommand
        );

    /// <summary>
    /// 菜单顶部按钮点击命令
    /// </summary>
    internal ICommand MenuTopButtonCommand { get; }

    #endregion
    #region MenuItemCommand

    internal static readonly DirectProperty<NavigationView, ICommand> MenuItemCommandProperty =
        AvaloniaProperty.RegisterDirect<NavigationView, ICommand>(
            nameof(MenuItemButtonCommand),
            o => o.MenuItemButtonCommand
        );

    internal ICommand MenuItemButtonCommand { get; }

    #endregion
    #region SelectedMenuItem

    public static readonly DirectProperty<NavigationView, MenuItemData?> SelectedMenuItemProperty =
        AvaloniaProperty.RegisterDirect<NavigationView, MenuItemData?>(
            nameof(SelectedMenuItem),
            o => o.SelectedMenuItem,
            (o, v) => o.SelectedMenuItem = v
        );
    private MenuItemData? _selectedMenuItem;
    public MenuItemData? SelectedMenuItem
    {
        get => _selectedMenuItem;
        set => SetAndRaise(SelectedMenuItemProperty, ref _selectedMenuItem, value);
    }
    #endregion

    #region DefaultSelectFirst

    /// <summary>
    /// <inheritdoc cref="DefaultSelectFirst"/>
    /// </summary>
    public static readonly DirectProperty<NavigationView, bool> DefaultSelectFirstProperty =
        AvaloniaProperty.RegisterDirect<NavigationView, bool>(
            nameof(DefaultSelectFirst),
            o => o.DefaultSelectFirst,
            (o, v) => o.DefaultSelectFirst = v,
            true
        );

    private bool _defaultSelectFirst = true;

    /// <summary>
    /// 当值为 <see cref="bool">true</see> 时，默认选中第一个菜单项
    /// </summary>
    public bool DefaultSelectFirst
    {
        get => _defaultSelectFirst;
        set => SetAndRaise(DefaultSelectFirstProperty, ref _defaultSelectFirst, value);
    }

    #endregion

    public NavigationView()
    {
        MenuTopButtonCommand = new RelayCommand(AfterMenuTopButtonClick);
        MenuItemButtonCommand = new RelayCommand<MenuItemData>(
            OnMenuItemButtonClick,
            d => Command?.CanExecute(d) ?? true
        );
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (DefaultSelectFirst)
        {
            SelectedMenuItem = MenuItemSource?.FirstOrDefault();
        }
    }

    private void AfterMenuTopButtonClick()
    {
        IsMenuOpen = !IsMenuOpen;
    }

    private void OnMenuItemButtonClick(MenuItemData? data)
    {
        SelectedMenuItem = data;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SelectedMenuItemProperty)
        {
            foreach (var item in MenuItemSource ?? [])
            {
                item.IsSelected = false;
            }
            if (SelectedMenuItem is not null)
            {
                SelectedMenuItem.IsSelected = true;
                Command?.Execute(SelectedMenuItem);
            }
        }
    }
}
