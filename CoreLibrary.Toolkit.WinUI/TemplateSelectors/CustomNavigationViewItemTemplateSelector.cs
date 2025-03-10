using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Toolkit.WinUI.Structs;
using CoreLibrary.Toolkit.WinUI.Templates;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CoreLibrary.Toolkit.WinUI.TemplateSelectors;

public class CustomNavigationViewItemTemplateSelector : DataTemplateSelector
{
    private readonly DataTemplate _defaultItemTemplate = new DefaultItemTemplate();
    private readonly DataTemplate _separatorItemTemplate = new SeparatorItemTemplate();
    private readonly DataTemplate _commandItemTemplate = new CommandItemTemplate();
    private readonly DataTemplate _asyncCommandItemTemplate = new AsyncCommandItemTemplate();

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        return item switch
        {
            PageItem => _defaultItemTemplate,
            SeparatorItem => _separatorItemTemplate,
            CommandItem => _commandItemTemplate,
            AsyncCommandItem => _asyncCommandItemTemplate,
            _ => throw new NotImplementedException(),
        };
    }
}
