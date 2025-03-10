using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CoreServicesWinUILibrary.TemplateSelectors;

internal sealed class SettingValueTemplateSelector : DataTemplateSelector
{
    public DataTemplate EmptyTemplate { get; set; } = new();

    public DataTemplate EnumTemplate { get; set; } = new();
    public DataTemplate StringTemplate { get; set; } = new();
    public DataTemplate NumberTemplate { get; set; } = new();
    public DataTemplate BoolTemplate { get; set; } = new();

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        Debug.WriteLine($"item:{item} container:{container}");
        if (item is SettingValueEnum)
        {
            return EnumTemplate;
        }
        else if (item is SettingValue sv)
        {
            switch (sv.Value)
            {
                case string:
                    return StringTemplate;
                case int
                or float
                or double
                or decimal:
                    return NumberTemplate;
                case bool:
                    return BoolTemplate;
                default:
                    break;
            }
        }
        return EmptyTemplate;
    }
}
