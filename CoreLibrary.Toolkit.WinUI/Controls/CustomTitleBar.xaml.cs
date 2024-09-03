using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Intrinsics.Arm;
using CommunityToolkit.WinUI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CoreLibrary.Toolkit.WinUI.Controls
{
    public sealed partial class CustomTitleBar : UserControl
    {
        #region Properties

        public static DependencyProperty TitleProperty { get; } = TextBlock.TextProperty;
        public static DependencyProperty SubTitleProperty { get; } = TextBlock.TextProperty;
        public static DependencyProperty TargetWindowProperty { get; } =
            DependencyProperty.Register(
                nameof(TargetWindow),
                typeof(Window),
                typeof(CustomTitleBar),
                new PropertyMetadata(null)
            );
        public string Title
        {
            get => (string)titleTextBlock.GetValue(TitleProperty);
            set
            {
                titleTextBlock.SetValue(TitleProperty, value);
                if (TargetWindow is not null)
                    TargetWindow.Title = value;
            }
        }
        public string SubTitle
        {
            get => (string)subTitleTextBlock.GetValue(SubTitleProperty);
            set => subTitleTextBlock.SetValue(SubTitleProperty, value);
        }
        public Window? TargetWindow
        {
            get => (Window?)GetValue(TargetWindowProperty);
            set
            {
                SetValue(TargetWindowProperty, value);
                if (value is not null)
                {
                    value.Title = Title;
                }
            }
        }
        #endregion


        public CustomTitleBar()
        {
            this.InitializeComponent();
        }

        #region 鼠标拖动代码（已废弃）

        //private bool _isDragging = false;
        //private System.Drawing.Point _pointerPos;
        //private global::Windows.Graphics.PointInt32 _windowPos;

        //protected override void OnPointerPressed(PointerRoutedEventArgs e)
        //{
        //    base.OnPointerPressed(e);
        //    if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        //    {
        //        _isDragging = true;
        //        _windowPos = TargetWindow?.AppWindow.Position ?? default;
        //        _pointerPos = GetCursorPos(out var point) ? point : default;
        //        CapturePointer(e.Pointer);
        //        e.Handled = true;
        //    }
        //}

        //protected override void OnPointerReleased(PointerRoutedEventArgs e)
        //{
        //    base.OnPointerReleased(e);
        //    ReleasePointerCapture(e.Pointer);
        //    _isDragging = false;
        //    e.Handled = true;
        //}

        //protected override void OnPointerMoved(PointerRoutedEventArgs e)
        //{
        //    base.OnPointerMoved(e);
        //    if (_isDragging)
        //    {
        //        GetCursorPos(out System.Drawing.Point pointerPos);
        //        TargetWindow?.AppWindow.Move(
        //            new(_windowPos.X + pointerPos.X - _pointerPos.X, _windowPos.Y + pointerPos.Y - _pointerPos.Y)
        //        );
        //    }
        //}

        ///// <summary>
        ///// 获取鼠标相对于屏幕的位置
        ///// </summary>
        ///// <param name="lpPoint"></param>
        ///// <returns></returns>
        //[LibraryImport("user32.dll")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //private static partial bool GetCursorPos(out System.Drawing.Point lpPoint);
        #endregion
    }
}
