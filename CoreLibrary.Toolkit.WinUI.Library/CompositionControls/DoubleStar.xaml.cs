using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Animations.Expressions;
using Microsoft.UI;
using Microsoft.UI.Composition;
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

namespace CoreServicesWinUILibrary.CompositionControls;

public sealed partial class DoubleStar : UserControl
{
    public DoubleStar()
    {
        this.InitializeComponent();
    }

    private CompositionPropertySet? _properties;

    internal void ToAnimation()
    {
        var compositor = this.GetVisual().Compositor;
        _properties = compositor.CreatePropertySet();
        _properties.InsertVector2("Position", new(200, 200));
        _properties.InsertScalar("Rotation", 0f);

        var background = compositor.CreateLinearGradientBrush();
        background.ColorStops.Add(compositor.CreateColorGradientStop(0, Colors.Red));
        background.ColorStops.Add(compositor.CreateColorGradientStop(1, Colors.Pink));
        background.StartAnimation(nameof(CompositionLinearGradientBrush.StartPoint), ExpressionFunctions.Vector2(0, 0));
        background.StartAnimation(nameof(CompositionLinearGradientBrush.EndPoint), ExpressionFunctions.Vector2(1, 1));
        var ellipse = compositor.CreateEllipseGeometry();
        ellipse.StartAnimation(
            nameof(CompositionEllipseGeometry.Center),
            _properties.GetReference().GetVector2Property("Position")
        );
        ellipse.Radius = new(50, 50);
        var ellipse2 = compositor.CreateEllipseGeometry();
        ellipse2.StartAnimation(
            nameof(CompositionEllipseGeometry.Center),
            _properties.GetReference().GetVector2Property("Position")
        );
        ellipse2.Radius = new(25, 25);
        var shape = compositor.CreateSpriteShape(ellipse);
        shape.FillBrush = background;
        var shape2 = compositor.CreateSpriteShape(ellipse2);
        shape2.FillBrush = background;
        var rotation = _properties.GetReference().GetScalarProperty("Rotation");
        shape2.StartAnimation(
            nameof(CompositionSpriteShape.Offset),
            ExpressionFunctions.Vector2(
                ExpressionFunctions.Cos(ExpressionFunctions.ToRadians(rotation)) * 150,
                ExpressionFunctions.Sin(ExpressionFunctions.ToRadians(rotation)) * 150
            )
        );
        var sprite = compositor.CreateShapeVisual();
        sprite.StartAnimation(nameof(Visual.Size), this.GetVisual().GetReference().Size);
        sprite.Shapes.Add(shape);
        sprite.Shapes.Add(shape2);
        ElementCompositionPreview.SetElementChildVisual(this, sprite);

        var keyFrameAnimation2 = compositor.CreateScalarKeyFrameAnimation();
        keyFrameAnimation2.InsertKeyFrame(0, 0);
        keyFrameAnimation2.InsertKeyFrame(1f, 360, compositor.CreateLinearEasingFunction());
        keyFrameAnimation2.Duration = TimeSpan.FromSeconds(2);
        keyFrameAnimation2.IterationBehavior = AnimationIterationBehavior.Forever;
        _properties.StartAnimation("Rotation", keyFrameAnimation2);

        Unloaded += (_, _) =>
        {
            keyFrameAnimation2?.Dispose();
        };
    }

    private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        Debug.WriteLine("Enter");
        var pos = e.GetCurrentPoint(this).Position;
        _properties?.InsertVector2("Position", new((float)pos.X, (float)pos.Y));
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ToAnimation();
    }
}
