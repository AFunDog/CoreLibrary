using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Toolkit.WinUI.Contracts;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CoreLibrary.Toolkit.WinUI.Services
{
    public class NavigateService : INavigateService
    {
        private Frame? _frame;

        public void AttachService(Frame frame)
        {
            _frame = frame;
            _frame.Unloaded += OnFrameUnloaded;
        }

        private void OnFrameUnloaded(object sender, RoutedEventArgs e)
        {
            _frame!.Unloaded -= OnFrameUnloaded;
            _frame = null;
        }

        public void Navigate(Type pageType, object? args = null)
        {
            _frame?.Navigate(pageType, args);
        }
    }
}
