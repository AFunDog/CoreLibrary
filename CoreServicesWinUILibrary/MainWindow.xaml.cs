using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CoreServices.WinUI.Common;
using CoreServices.WinUI.Contracts;
using CoreServices.WinUI.Extensions;
using CoreServices.WinUI.Structs;
using CoreServicesWinUILibrary.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CoreServicesWinUILibrary
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public static MainWindow? Instance { get; private set; }

        public ICollection<IPageItem> HeaderPageItems { get; } =
            [
                new PageItem(
                    "主页",
                    "\uE80F",
                    typeof(MainPage),
                    new NavigateTransitionSelector(t =>
                    {
                        if (t == typeof(SettingPage))
                            return new SlideNavigationTransitionInfo()
                            {
                                Effect = SlideNavigationTransitionEffect.FromRight
                            };
                        return null;
                    })
                ),
                new SeparatorItem(),
                new PageItem("控件", "\uE74C", typeof(ControlsPage)),
                new PageItem("动画", "\uE768", typeof(AnimationPage)),
            ];
        public ICollection<IPageItem> FooterPageItems { get; } =
            [
                new SeparatorItem(),
                new PageItem(
                    "设置",
                    "\uE713",
                    typeof(SettingPage),
                    new NavigateTransitionSelector(t =>
                    {
                        if (t == typeof(MainPage))
                            return new SlideNavigationTransitionInfo()
                            {
                                Effect = SlideNavigationTransitionEffect.FromLeft
                            };
                        return null;
                    })
                )
            ];

        public MainWindow()
        {
            Instance = this;
            this.SetNoneWindowStyle();
            this.SetWindowSize(1280, 720);
            this.InitializeComponent();

            WindowNavigationView.Loaded += (s, e) =>
            {
                App.GetService<INavigateService>().AttachService(WindowNavigationView.ContentFrame);
                App.GetService<INavigateService>().Navigate(typeof(MainPage));
            };

            Closed += (s, e) =>
            {
                Instance = null;
            };
        }
    }
}
