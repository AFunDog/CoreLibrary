using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Vanara;
using Vanara.PInvoke;

namespace CoreLibrary.Toolkit.WinUI.Extensions
{
    public static partial class WindowExtension
    {
        /// <summary>
        /// 设置窗体无标题栏，不可最大化、最小化和调整尺寸，无关闭按钮
        /// </summary>
        /// <param name="window"></param>
        public static void SetNoneWindowStyle(this Window window)
        {
            if (window.AppWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.IsMaximizable = false;
                presenter.IsMinimizable = false;
                presenter.IsResizable = false;
                presenter.SetBorderAndTitleBar(false, false);
            }
        }
    }
}
