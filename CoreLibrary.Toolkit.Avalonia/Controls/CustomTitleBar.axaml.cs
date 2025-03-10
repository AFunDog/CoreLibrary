using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Metadata;
using CommunityToolkit.Mvvm.Input;

namespace CoreLibrary.Toolkit.Avalonia.Controls;

[TemplatePart("PART_Maximize", typeof(Button))]
[TemplatePart("PART_Minimize", typeof(Button))]
[TemplatePart("PART_Close", typeof(Button))]
[PseudoClasses(":maximized")]
public class CustomTitleBar : TemplatedControl
{
    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<CustomTitleBar, string>(
        nameof(Title)
    );

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly StyledProperty<Control?> MiddleContentProperty = AvaloniaProperty.Register<
        CustomTitleBar,
        Control?
    >(nameof(MiddleContent));

    [Content]
    public Control? MiddleContent
    {
        get => GetValue(MiddleContentProperty);
        set => SetValue(MiddleContentProperty, value);
    }

    #region Command


    public static readonly DirectProperty<CustomTitleBar, ICommand> MinimizeCommandProperty =
        AvaloniaProperty.RegisterDirect<CustomTitleBar, ICommand>(nameof(MinimizeCommand), o => o.MinimizeCommand);

    public ICommand MinimizeCommand { get; }

    public static readonly DirectProperty<CustomTitleBar, ICommand> MaximizeCommandProperty =
        AvaloniaProperty.RegisterDirect<CustomTitleBar, ICommand>(nameof(MaximizeCommand), o => o.MaximizeCommand);

    public ICommand MaximizeCommand { get; }

    public static readonly DirectProperty<CustomTitleBar, ICommand> CloseCommandProperty =
        AvaloniaProperty.RegisterDirect<CustomTitleBar, ICommand>(nameof(CloseCommand), o => o.CloseCommand);

    public ICommand CloseCommand { get; }

    #endregion
    #region ButtonVisible

    public static readonly StyledProperty<bool> IsMaximizeButtonVisibleProperty = AvaloniaProperty.Register<
        CustomTitleBar,
        bool
    >(nameof(IsMaximizeButtonVisible), true);

    public bool IsMaximizeButtonVisible
    {
        get => GetValue(IsMaximizeButtonVisibleProperty);
        set => SetValue(IsMaximizeButtonVisibleProperty, value);
    }

    public static readonly StyledProperty<bool> IsMinimizeButtonVisibleProperty = AvaloniaProperty.Register<
        CustomTitleBar,
        bool
    >(nameof(IsMinimizeButtonVisible), true);

    public bool IsMinimizeButtonVisible
    {
        get => GetValue(IsMinimizeButtonVisibleProperty);
        set => SetValue(IsMinimizeButtonVisibleProperty, value);
    }

    public static readonly StyledProperty<bool> IsCloseButtonVisibleProperty = AvaloniaProperty.Register<
        CustomTitleBar,
        bool
    >(nameof(IsCloseButtonVisible), true);

    public bool IsCloseButtonVisible
    {
        get => GetValue(IsCloseButtonVisibleProperty);
        set => SetValue(IsCloseButtonVisibleProperty, value);
    }

    #endregion

    public static readonly StyledProperty<bool> IsHideOnCloseProperty = AvaloniaProperty.Register<CustomTitleBar, bool>(
        nameof(IsHideOnClose),
        false
    );

    public bool IsHideOnClose
    {
        get => GetValue(IsHideOnCloseProperty);
        set => SetValue(IsHideOnCloseProperty, value);
    }

    private Window? TargetWindow { get; set; }

    public CustomTitleBar()
    {
        MinimizeCommand = new RelayCommand(AfterMinimizeClick);
        MaximizeCommand = new RelayCommand(AfterMaximizeClick);
        CloseCommand = new RelayCommand(AfterCloseClick);
    }

    protected override void OnInitialized()
    {
        TargetWindow = this.FindLogicalAncestorOfType<Window>();
        if (TargetWindow is not null)
        {
            TargetWindow.PropertyChanged += (s, e) =>
            {
                if (e.Property == Window.WindowStateProperty)
                {
                    switch (TargetWindow.WindowState)
                    {
                        case WindowState.Maximized:
                            PseudoClasses.Add(":maximized");
                            break;
                        default:
                            PseudoClasses.Remove(":maximized");
                            break;
                    }
                }
            };
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            TargetWindow?.BeginMoveDrag(e);
            e.Handled = true;
        }
    }

    private void AfterMinimizeClick()
    {
        if (TargetWindow is not null)
        {
            TargetWindow.WindowState = WindowState.Minimized;
        }
    }

    private void AfterMaximizeClick()
    {
        if (TargetWindow is not null)
        {
            if (TargetWindow.WindowState == WindowState.Maximized)
            {
                TargetWindow.WindowState = WindowState.Normal;
            }
            else
            {
                TargetWindow.WindowState = WindowState.Maximized;
            }
        }
    }

    private void AfterCloseClick()
    {
        if (IsHideOnClose)
        {
            TargetWindow?.Hide();
        }
        else
        {
            TargetWindow?.Close();
        }
    }
}
