using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Toolkit.WinUI.Contracts;
using Microsoft.UI.Xaml.Media.Animation;

namespace CoreLibrary.Toolkit.WinUI.Common
{
    public class NavigateTransitionSelector(Func<Type, NavigationTransitionInfo?>? selectAction = null)
        : INavigateTransitionSelector
    {
        private readonly Func<Type, NavigationTransitionInfo?>? _selectAction = selectAction;

        public NavigationTransitionInfo? GetTransition(Type targetPageType)
        {
            return _selectAction?.Invoke(targetPageType);
        }
    }
}
