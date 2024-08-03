using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreServices.WinUI.Contracts;
using Microsoft.UI.Xaml;

namespace CoreServices.WinUI.Structs
{
    public sealed record PageItem(string Title,string IconGlyph,Type PageType,INavigateTransitionSelector? TransitionSelector = null)  : IPageItem
    {
    }
}
