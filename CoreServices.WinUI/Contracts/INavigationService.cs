using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreServices.WinUI.Contracts
{
    public interface INavigationService
    {
        void Register(NavigationView navigationView,Frame frame);

        void Navigate(Type pageType, object? args = null);
    }
}
