using CoreServices.WinUI.Structs;
using CoreServices.WinUI.Templates;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreServices.WinUI.TemplateSelectors
{
    public class CustomNavigationViewItemTemplateSelector : DataTemplateSelector
    {
        private readonly DataTemplate _defaultItemTemplate = new DefaultItemTemplate();
        private readonly DataTemplate _separatorItemTemplate = new SeparatorItemTemplate();

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return (item is PageItem) ? _defaultItemTemplate: _separatorItemTemplate;
        }
    }
}
