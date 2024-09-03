using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Animations;
using CommunityToolkit.WinUI.Animations.Expressions;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CoreServicesWinUILibrary.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AnimationPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public int WindowWidth
        {
            get => MainWindow.Instance!.AppWindow.Size.Width;
            set
            {
                MainWindow.Instance!.AppWindow.Resize(MainWindow.Instance!.AppWindow.Size with { Width = value });
                PropertyChanged?.Invoke(this, new(nameof(WindowWidth)));
            }
        }

        public int WindowHeight
        {
            get => MainWindow.Instance!.AppWindow.Size.Height;
            set
            {
                MainWindow.Instance!.AppWindow.Resize(MainWindow.Instance!.AppWindow.Size with { Height = value });
                PropertyChanged?.Invoke(this, new(nameof(WindowHeight)));
            }
        }

        public AnimationPage()
        {
            this.InitializeComponent();
        }
    }
}
