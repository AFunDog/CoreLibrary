using CoreServices.WinUI.Contracts;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreServices.WinUI.Services
{
    public class NavigateService : INavigateService
    {
        private Frame? _frame;
        public void AttachService(Frame frame)
        {
            _frame = frame;
        }

        public void Navigate(Type pageType, object? args = null)
        {
            _frame?.Navigate(pageType, args);
        }
    }
}
