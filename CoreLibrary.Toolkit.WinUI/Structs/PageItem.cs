using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Toolkit.WinUI.Contracts;
using Microsoft.UI.Xaml;

namespace CoreLibrary.Toolkit.WinUI.Structs
{
    public sealed record PageItem(
        string Title,
        string IconGlyph,
        Type PageType,
        INavigateTransitionSelector? TransitionSelector = null
    ) : IPageItem { }
}
