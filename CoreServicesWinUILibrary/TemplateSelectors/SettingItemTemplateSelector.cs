using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreServicesWinUILibrary.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Graphics.Display;

namespace CoreServicesWinUILibrary.TemplateSelectors
{
    internal sealed class SettingItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SettingCardTemplate { get; set; } = new();
        public DataTemplate SettingExpanderTemplate { get; set; } = new();

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var settingNodeInfo = (item as SettingNodeInfo)!;
            if (settingNodeInfo.Kids.Count > 0)
            {
                return SettingExpanderTemplate;
            }
            else
            {
                return SettingCardTemplate;
            }
        }
    }
}
