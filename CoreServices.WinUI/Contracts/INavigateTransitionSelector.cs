using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreServices.WinUI.Contracts
{
    public interface INavigateTransitionSelector
    {
        NavigationTransitionInfo? GetTransition(Type targetPageType);
    }
}
