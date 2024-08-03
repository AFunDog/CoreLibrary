using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using CommunityToolkit.WinUI;
using CoreServices.WinUI.Animation;
using CoreServices.WinUI.Extensions;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CoreServices.WinUI.Controls
{
    public sealed partial class CircleProgress : Control, INotifyPropertyChanged
    {
        #region 属性

        public static DependencyProperty ValueProperty { get; } =
            DependencyProperty.Register(
                nameof(Value),
                typeof(double),
                typeof(CircleProgress),
                new PropertyMetadata(0d, OnValuePropertyChanged)
            );
        public static DependencyProperty ProgressFillBrushProperty { get; } =
            DependencyProperty.Register(
                nameof(ProgressFillBrush),
                typeof(Brush),
                typeof(CircleProgress),
                new PropertyMetadata(new SolidColorBrush(Colors.Coral))
            );
        public static DependencyProperty IsShowValueProperty { get; } =
            DependencyProperty.Register(
                nameof(IsShowValue),
                typeof(bool),
                typeof(CircleProgress),
                new PropertyMetadata(true, OnIsShowValuePropertyChanged)
            );
        public static DependencyProperty TitleProperty { get; } =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(CircleProgress),
                new PropertyMetadata(string.Empty)
            );
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public Brush ProgressFillBrush
        {
            get => (Brush)GetValue(ProgressFillBrushProperty);
            set => SetValue(ProgressFillBrushProperty, value);
        }
        public bool IsShowValue
        {
            get => (bool)GetValue(IsShowValueProperty);
            set => SetValue(IsShowValueProperty, value);
        }
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        private double AnimationValue { get; set; }

        internal Visibility ValueTextVisibility => IsShowValue ? Visibility.Visible : Visibility.Collapsed;

        #endregion
        #region 绘图点

        //private CompositionPropertySet _pointProperties;
        //private readonly ScalarKeyFrameAnimation _angleAnimation;

        internal float Angle => MathF.PI * ((1f / 6f) - ((float)AnimationValue / 100f) * (240f / 180f));

        internal Point RightPoint1 => new(200f - 180f * MathF.Cos(Angle), 200f + 180f * MathF.Sin(Angle));
        internal Point RightPoint2 => new(200f - 160f * MathF.Cos(Angle), 200f + 160f * MathF.Sin(Angle));

        internal bool IsLargeArc => Angle <= -MathF.PI * (5f / 6f);

        private void UpdatePoints()
        {
            PropertyChanged?.Invoke(this, new(nameof(RightPoint1)));
            PropertyChanged?.Invoke(this, new(nameof(RightPoint2)));
            PropertyChanged?.Invoke(this, new(nameof(IsLargeArc)));
        }

        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly PropertyAnimation<double> _valueAnimation;

        public CircleProgress()
        {
            this.InitializeComponent();
            _valueAnimation = new PropertyAnimation<double>(
                target: this,
                propertyName: nameof(AnimationValue),
                from: 0,
                to: Value,
                converter: e =>
                {
                    UpdatePoints();
                    return e.From + (e.to - e.From) * e.Progress;
                },
                easingType: CommunityToolkit.WinUI.Animations.EasingType.Cubic,
                easingMode: Microsoft.UI.Xaml.Media.Animation.EasingMode.EaseOut
            );
            Width = Math.Min(MinWidth, Width);
            Height = Math.Min(MinHeight, Height);
        }

        private static void OnIsShowValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (d as CircleProgress)!;
            source.PropertyChanged?.Invoke(d, new(nameof(IsShowValue)));
            source.PropertyChanged?.Invoke(d, new(nameof(ValueTextVisibility)));
        }

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (d as CircleProgress)!;
            source._valueAnimation.StartAnimation();
            source._valueAnimation.UpdateToValue((double)e.NewValue);
            source.PropertyChanged?.Invoke(source, new(nameof(Value)));
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Width = Math.Min(MinWidth, e.NewSize.Width);
            Height = Math.Min(MinHeight, e.NewSize.Height);
        }
    }
}
