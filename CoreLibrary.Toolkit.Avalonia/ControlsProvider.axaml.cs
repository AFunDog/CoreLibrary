using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CoreLibrary.Toolkit.Avalonia;

public sealed class ControlsProvider : ResourceDictionary
{
    public ControlsProvider()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
