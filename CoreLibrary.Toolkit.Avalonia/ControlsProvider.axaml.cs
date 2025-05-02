using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Zeng.CoreLibrary.Toolkit.Avalonia;

public sealed class ControlsProvider : ResourceDictionary
{
    public ControlsProvider()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
