using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Animation;

namespace CoreLibrary.Toolkit.WinUI.Contracts
{
    public interface INavigateTransitionSelector
    {
        NavigationTransitionInfo? GetTransition(Type targetPageType);
    }
}
